﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private Thread cpuThread;
        private double[] gsrArray = new double[60];        
        private double[] tempArray = new double[60];
        private double[] respArray = new double[60];
        //Bochecha
        private double[] emg1Array = new double[60];
        //Testa
        private double[] emg2Array = new double[60];
        private double[] hrArray = new double[60];


        // {GSR, Temp, Resp, EMG1, EMG2, HR}
        private double[] minValues = new double[6];
        private double[] maxValues = new double[6];



        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 6; i++)
            {
                minValues[i] = Double.MaxValue;
                maxValues[i] = Double.MinValue;
            }
        }

        private void getPerformanceCounters()
        {
           
            while (true)
            {
                FileStream fs = new FileStream("C:/BioTrace/System/DataChan.bin", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                //FileStream fs = new FileStream("C:/Users/V/Desktop/test.bin", FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs, Encoding.ASCII);

                //4 - GSR
                //5 - Temp
                //7 - Resp
                //12 - EMG 1
                //17 - EMG 2
                //22 - Heart Rate


                for (int i = 0; i < 23; i++)
                {
                    switch (i)
                    {
                        case 4:
                            gsrArray[gsrArray.Length - 1] = br.ReadDouble();
                            minValues[0] = Math.Min(minValues[0], gsrArray[gsrArray.Length - 1]);
                            maxValues[0] = Math.Max(maxValues[0], gsrArray[gsrArray.Length - 1]);
                            break;
                        case 5:
                            tempArray[tempArray.Length - 1] = br.ReadDouble();
                            minValues[1] = Math.Min(minValues[1], tempArray[gsrArray.Length - 1]);
                            maxValues[1] = Math.Max(maxValues[1], tempArray[gsrArray.Length - 1]);
                            break;
                        case 7:
                            respArray[gsrArray.Length - 1] = br.ReadDouble();
                            minValues[2] = Math.Min(minValues[2], respArray[gsrArray.Length - 1]);
                            maxValues[2] = Math.Max(maxValues[2], respArray[gsrArray.Length - 1]);
                            break;
                        case 12:
                            emg1Array[gsrArray.Length - 1] = br.ReadDouble();
                            minValues[3] = Math.Min(minValues[3], emg1Array[gsrArray.Length - 1]);
                            maxValues[3] = Math.Max(maxValues[3], emg1Array[gsrArray.Length - 1]);
                            break;
                        case 17:
                            emg2Array[gsrArray.Length - 1] = br.ReadDouble();
                            minValues[4] = Math.Min(minValues[4], emg2Array[gsrArray.Length - 1]);
                            maxValues[4] = Math.Max(maxValues[4], emg2Array[gsrArray.Length - 1]);
                            break;
                        case 22:
                            hrArray[gsrArray.Length - 1] = br.ReadDouble();
                            minValues[5] = Math.Min(minValues[5], hrArray[gsrArray.Length - 1]);
                            maxValues[5] = Math.Max(maxValues[5], hrArray[gsrArray.Length - 1]);
                            break;
                        default:
                            br.ReadDouble();
                            break;
                    }

                }

                Array.Copy(gsrArray, 1, gsrArray, 0, gsrArray.Length - 1);
                Array.Copy(tempArray, 1, tempArray, 0, tempArray.Length - 1);
                Array.Copy(respArray, 1, respArray, 0, respArray.Length - 1);
                Array.Copy(emg1Array, 1, emg1Array, 0, emg1Array.Length - 1);
                Array.Copy(emg2Array, 1, emg2Array, 0, emg2Array.Length - 1);
                Array.Copy(hrArray, 1, hrArray, 0, hrArray.Length - 1);


                if (chart1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateCpuChart(); });
                }
                else
                {
                    //......
                }

                Thread.Sleep(1000);
            }
        }

        private void UpdateCpuChart()
        {
            chart1.Series["EMG1"].Points.Clear();
            chart1.Series["EMG2"].Points.Clear();
            chart2.Series["Resp"].Points.Clear();
            chart2.Series["HR"].Points.Clear();
            chart3.Series["GSR"].Points.Clear();
            chart3.Series["Temp"].Points.Clear();

            for (int i = 0; i < gsrArray.Length - 1; ++i)
            {
                chart1.Series["EMG1"].Points.AddY(emg1Array[i]);
                chart1.Series["EMG2"].Points.AddY(emg2Array[i]);
                chart2.Series["Resp"].Points.AddY(respArray[i]);
                chart2.Series["HR"].Points.AddY(hrArray[i]);
                chart3.Series["GSR"].Points.AddY(gsrArray[i]);
                chart3.Series["Temp"].Points.AddY(tempArray[i]);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            cpuThread = new Thread(new ThreadStart(this.getPerformanceCounters));
            cpuThread.IsBackground = true;
            cpuThread.Start();
        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image  
            // assigned to Button2.  
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "*Text Files | *.txt";
            saveFileDialog1.Title = "Save values";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.  
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveFileDialog1.OpenFile();
                

                for(int i = 0; i < maxValues.Length; i++)
                {
                    string s = maxValues[i] + ";" + minValues[i] + "\n";
                    byte[] arr = Encoding.ASCII.GetBytes(s);
                    fs.Write(arr, 0, arr.Length);
                }




                fs.Close();
            }
        }
    }
}

