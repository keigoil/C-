using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestRandomness
{
    class Program
    {
        int N;
        int n;
        int m;
        
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        private void Run()
        {
            N = 10000;
            n = 10;
            m = 5;

            int[] intArr = new int[n];
            DateTime startTime = DateTime.Now;

            for (int i = 0; i < N; i++)
            {
                var arr1 = CreatMines(intArr);
            }

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("{0}:{1}",i + 1,(float)intArr[i] / N);
            }
            
            Console.WriteLine("消耗时间:{0}秒！", (startTime - DateTime.Now).Duration().TotalSeconds);
            
            Console.ReadLine();
        }

        private int[] CreatMines(int[] intArr)
        {
            var arr = new int[n];

            int i = 0;
            DateTime aa = DateTime.Now;
            while (i < m)
            {
                Random radom = new Random();
                int index = radom.Next(0, n);
                if(arr[index] != 1)
                {
                    arr[index] = 1;
                    intArr[index] += 1;
                    i++;
                }
            }

            DateTime bb = DateTime.Now;
            var cc = (aa - bb).Duration().TotalSeconds;

            return arr;
        }

        private int[] CreatMines2(int[] intArr)
        {
            int[] arr = new int[n];
            
            //前5个值为1
            for (int i = 0; i < m; i++)
            {
                arr[i] = 1;
            }

            for (int i = 0; i < m; i++)
            {
                var dateTime = DateTime.Now;
                Random radom = new Random((int)dateTime.Ticks);
                var index = radom.Next(0, n);

                var number = arr[i];
                arr[i] = arr[index];
                arr[index] = number;

                
            }

            for (int i = 0; i < n; i++)
            {
                intArr[i] += arr[i];
            }

            return arr;
        }
    }
}
