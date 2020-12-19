using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// 
/// </summary>
namespace MineClearance
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 旗子index
        /// </summary>
        HashSet<int> _flagIndexHash = new HashSet<int>();
        /// <summary>
        /// 地雷index
        /// </summary>
        HashSet<int> _bombIndexHash = new HashSet<int>();
        /// <summary>
        /// 是否是第一次点击按钮
        /// </summary>
        bool _flag = true;
        /// <summary>
        /// 已经确认有数字的按钮
        /// </summary>
        int _confirmedBtnCount = 0;

        /// <summary>
        /// 是否继续递归
        /// </summary>
        bool _canRecursion = true;

        /// <summary>
        /// 鼠标按下时间
        /// </summary>
        DateTime _leftClickTime = new DateTime();
        DateTime _rightClickTime = new DateTime();
        DateTime _firstClickTime = new DateTime();

        /// <summary>
        /// 可设置
        /// </summary>
        int _rows = 20;
        int _colums = 30;
        int _bombCount = 60;

        MainWindowViewModel _vm;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _vm = new MainWindowViewModel();
            this.Width = _colums * 40 + 16;
            this.Height = _rows * 40 + 64;

            BeforeStartGame();
        }

        #region 点击前游戏的准备工作

        /// <summary>
        /// 开始游戏前
        /// </summary>
        private void BeforeStartGame()
        {
            GameBG.Children.Clear();
            _flag = true;
            _confirmedBtnCount = 0;
            FaceBtn.Content = "笑脸";
            _bombIndexHash = new HashSet<int>();
            _flagIndexHash = new HashSet<int>();
            _firstClickTime = new DateTime();

            _vm.Remainder = _bombCount;
            //创建地图
            CreateMap(_rows, _colums);
            //填充按钮
            FillBtn(_rows, _colums);
            //剩余地雷数目
            _vm.Remainder = _bombCount;
        }

        /// <summary>
        /// 创建地图
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="colums"></param>
        private void CreateMap(int rows, int colums)
        {
            for (int i = 0; i < rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(40);
                GameBG.RowDefinitions.Add(row);
            }

            for (int j = 0; j < colums; j++)
            {
                ColumnDefinition colum = new ColumnDefinition();
                colum.Width = new GridLength(40);
                GameBG.ColumnDefinitions.Add(colum);
            }
            //设置网格线在游戏区域中可见
            GameBG.ShowGridLines = true;
        }

        /// <summary>
        /// 填充按钮
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="colums"></param>
        private void FillBtn(int rows, int colums)
        {
            var index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < colums; j++)
                {
                    //按钮属性赋值、方法绑定、位置设定
                    Button btn = new Button();
                    btn.Background = Brushes.Blue;
                    btn.Click += new RoutedEventHandler(Btn_LeftClick);
                    btn.MouseRightButtonUp += Btn_MouseRightButtonUp;
                    //btn.MouseDoubleClick += Btn_MouseDoubleClick;
                    btn.Tag = new BtnInfo(i, j, index, CreateAroundList(i, j), false);
                    GameBG.Children.Add(btn);
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);

                    index++;
                }
            }
        }
        #endregion

        #region 游戏点击功能
        /// <summary>
        /// 左键点击按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LeftClick(object sender, RoutedEventArgs e)
        {
            _leftClickTime = DateTime.Now;

            //如果左右键同时按下
            if (DeterminePressMeanWhile(_leftClickTime, _rightClickTime) && !_flag)
            {
                DoLeftRightPress(sender);
                return;
            }

            var btn = sender as Button;
            var btnInfo = btn.Tag as BtnInfo;

            if (btn.Content != null && btn.Content.ToString() == "旗子") return;


            if (_flag)
            {
                _canRecursion = true;
                //放雷
                SetBomb(btnInfo.Index);

                TraversButton(btnInfo.Index);

                //取消第一次点击判断
                _flag = false;

                _firstClickTime = DateTime.Now;
            }
            else
            {
                if (btnInfo.IsBomb)
                {
                    ClickBomb(btn);
                }
                else
                {
                    TraversButton(btnInfo.Index);
                }

                btnInfo.IsChecked = true;
            }

            IsGameEnd();
        }

        /// <summary>
        /// 右键点击按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _rightClickTime = DateTime.Now;

            //如果左右键同时按下
            if (DeterminePressMeanWhile(_leftClickTime, _rightClickTime) && !_flag)
            {
                DoLeftRightPress(sender);
                return;
            }

            var btn = sender as Button;
            var btnInfo = btn.Tag as BtnInfo;

            if (btnInfo.IsChecked) return;

            if (btn.Content == null || btn.Content.ToString() == "")
            {
                btn.Content = "旗子";
                _flagIndexHash.Add(btnInfo.Index);
                _vm.Remainder--;
                btn.Background = Brushes.White;
            }
            else if (btn.Content.ToString() == "旗子")
            {
                btn.Content = "";
                _flagIndexHash.Remove(btnInfo.Index);
                _vm.Remainder++;
                btn.Background = Brushes.Blue;
            }

            //取消第一次点击判断
            if (_flag)
            {
                _canRecursion = true;
                _firstClickTime = DateTime.Now;
                _flag = false;
            }

            IsGameEnd();
        }

        /// <summary>
        /// 点击中间笑脸按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FaceBtn_Click(object sender, RoutedEventArgs e)
        {
            BeforeStartGame();
        }
        
        /// <summary>
        /// 帮助按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("帮助？啸惹！");
        }

        /// <summary>
        /// 难度选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            DifficultySelectionPopup pop = new DifficultySelectionPopup();
            pop.ShowDialog();
        }

        #endregion

        #region 实现
        /// <summary>
        /// 点到了地雷
        /// </summary>
        /// <param name="btn"></param>
        private void ClickBomb(Button btn)
        {
            FaceBtn.Content = "哭脸";

            //遍历所有的地雷
            foreach (var item in _bombIndexHash)
            {
                var btn1 = GameBG.Children[item] as Button;
                //如果有用旗子做标识的，则保持原样，没有标识的，则标出雷并背景置灰
                if (btn1.Content != null && btn1.Content.ToString() == "旗子")
                {
                    _flagIndexHash.Remove((btn1.Tag as BtnInfo).Index);
                }
                else
                {
                    btn1.Content = "雷";
                    btn1.Background = Brushes.Gray;
                }
            }

            //标旗子并序号不在雷中的，则为标错的，背景置红
            if (_flagIndexHash.Count != 0)
            {
                foreach (var item in _flagIndexHash)
                {
                    var btn1 = GameBG.Children[item] as Button;
                    btn1.Background = Brushes.Red;
                }
            }

            btn.Background = Brushes.Red;

            if (MessageBox.Show("是否重新开始？") == MessageBoxResult.OK)
            {
                BeforeStartGame();
            }
        }

        /// <summary>
        /// 递归遍历空白按钮
        /// </summary>
        /// <param name="index"></param>
        private void TraversButton(int index)
        {
            if (!_canRecursion) return;

            var btn = (GameBG.Children[index] as Button);
            var btnInfo = btn.Tag as BtnInfo;

            btn.Background = Brushes.White;

            if (btnInfo.IsBomb)
            {
                _canRecursion = false;
                ClickBomb(btn);
                return;
            }

            //该按钮没有被显示过
            if (!btnInfo.IsChecked)
            {
                //计算周围地雷数量
                btnInfo.BombCount = GetBtnBombCount(btnInfo.Index);
                btnInfo.IsChecked = true;

                //空白按钮则继续遍历
                if (btnInfo.BombCount == 0)
                {
                    for (int i = 0; i < btnInfo.AroundIndexHash.Count; i++)
                    {
                        TraversButton(btnInfo.AroundIndexHash.ElementAt(i));
                    }
                }
                else
                {
                    btn.Content = btnInfo.BombCount;
                }

                if (!_canRecursion) return;

                _confirmedBtnCount++;

                IsGameEnd();
            }

        }

        /// <summary>
        /// 放置地雷
        /// </summary>
        /// <param name="num"></param>
        private void SetBomb(int index)
        {
            Random ran = new Random();

            //HashSet无重复
            while (_bombIndexHash.Count < _bombCount)
            {
                int bombIndex = ran.Next(0, _rows * _colums - 1);
                if (bombIndex != index)
                {
                    _bombIndexHash.Add(bombIndex);
                    ((GameBG.Children[bombIndex] as Button).Tag as BtnInfo).IsBomb = true;
                }
            }
        }

        /// <summary>
        /// 获取按钮周围地雷数量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetBtnBombCount(int index)
        {
            var info = (GameBG.Children[index] as Button).Tag as BtnInfo;

            var count = 0;
            for (int i = 0; i < info.AroundIndexHash.Count; i++)
            {
                if (((GameBG.Children[info.AroundIndexHash.ElementAt(i)] as Button).Tag as BtnInfo).IsBomb)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 创建周围按钮的index的列表
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colum"></param>
        /// <returns></returns>
        private HashSet<int> CreateAroundList(int row, int colum)
        {
            HashSet<int> aroundIndex = new HashSet<int>();
            //1
            var index = IsInMap(row - 1, colum - 1);
            if (index != -1)
            {
                aroundIndex.Add(index);
            }
            //2
            index = IsInMap(row - 1, colum);
            if (index != -1)
            {
                aroundIndex.Add(index);
            }
            //3
            index = IsInMap(row - 1, colum + 1);
            if (index != -1)
            {
                aroundIndex.Add(index);
            }
            //4
            index = IsInMap(row, colum - 1);
            if (index != -1)
            {
                aroundIndex.Add(index);
            }
            //var index = IsInMap(row, colum);
            //if (index != -1)
            //{
            //    aroundIndex.Add(index);
            //}
            //6
            index = IsInMap(row, colum + 1);
            if (index != -1)
            {
                aroundIndex.Add(index);
            }
            //7
            index = IsInMap(row + 1, colum - 1);
            if (index != -1)
            {
                aroundIndex.Add(index);
            }
            //8
            index = IsInMap(row + 1, colum);
            if (index != -1)
            {
                aroundIndex.Add(index);
            }
            //9
            index = IsInMap(row + 1, colum + 1);
            if (index != -1)
            {
                aroundIndex.Add(index);
            }
            return aroundIndex;
        }

        /// <summary>
        /// 判断该坐标是否在地图内，在则返回坐标的index，不在则返回-1
        /// </summary>
        /// <param name="row"></param>
        /// <param name="colum"></param>
        /// <returns></returns>
        private int IsInMap(int row, int colum)
        {
            if (row > -1 && row < _rows && colum > -1 && colum < _colums)
            {
                return row * _colums + colum;
            }

            return -1;
        }

        /// <summary>
        /// 判断左右键是否同时按下
        /// </summary>
        private bool DeterminePressMeanWhile(DateTime left, DateTime right)
        {
            TimeSpan ts = (left - right).Duration();
            if (ts.TotalSeconds <= 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 执行左右键同时按的方法
        /// </summary>
        private void DoLeftRightPress(object sender)
        {
            var btn = sender as Button;
            var btnInfo = btn.Tag as BtnInfo;

            if (btn.Content != null && btn.Content.ToString() == "旗子") return;

            //获取周围旗子序号
            var flagList = _flagIndexHash.Intersect(btnInfo.AroundIndexHash).ToList();
            if (flagList.Count == 0) return;

            //遍历周围按钮
            for (int i = 0; i < btnInfo.AroundIndexHash.Count; i++)
            {
                if (!_canRecursion)
                {
                    return;
                }
                var index = btnInfo.AroundIndexHash.ElementAt(i);
                //如果按钮是旗子则跳过，不是旗子继续执行
                if (!flagList.Contains(index))
                {
                    //如果是炸弹
                    if (((GameBG.Children[index] as Button).Tag as BtnInfo).IsBomb)
                    {
                        ClickBomb(GameBG.Children[index] as Button);
                        return;
                    }
                    else
                    {
                        TraversButton(index);
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否游戏结束
        /// </summary>
        private void IsGameEnd()
        {
            if (_confirmedBtnCount + _flagIndexHash.Count == _rows * _colums)
            {
                _canRecursion = false;
                var dateNow = DateTime.Now;
                var timeSpan = (dateNow - _firstClickTime).Duration();
                MessageBox.Show("成功通关，时间为：" + (int)timeSpan.TotalSeconds + "秒！");

                if (MessageBox.Show("是否重新开始？") == MessageBoxResult.OK)
                {
                    BeforeStartGame();
                }
            }
        }

        #endregion

        /// <summary>
        /// 按钮信息
        /// </summary>
        public class BtnInfo
        {
            public BtnInfo(int rows, int colums, int index, HashSet<int> aroundIndexHash, bool isBomb = false)
            {
                Row = rows;
                Colum = colums;
                Index = index;
                AroundIndexHash = aroundIndexHash;
                IsBomb = isBomb;
                IsChecked = false;
            }
            /// <summary>
            /// 行
            /// </summary>
            public int Row { get; set; }
            /// <summary>
            /// 列
            /// </summary>
            public int Colum { get; set; }
            /// <summary>
            /// 在所有按钮中的序号
            /// </summary>
            public int Index { get; set; }
            /// <summary>
            /// 周围地雷数量
            /// </summary>
            public int BombCount { get; set; }
            /// <summary>
            /// 周围8个按钮序号
            /// </summary>
            public HashSet<int> AroundIndexHash { get; set; }
            /// <summary>
            /// 是否是地雷
            /// </summary>
            public bool IsBomb { get; set; }
            /// <summary>
            /// 是否已被显示过
            /// </summary>
            public bool IsChecked { get; set; }
        }

        
    }
}
