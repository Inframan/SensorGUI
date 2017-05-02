using System;
using System.IO;
using System.Text;

public class SensorReader
{
    public const String FILE_PATH = "C:/Users/V/Desktop/DataChan.bin";
    private double[] buffer = new double[161];
    public double read(String filePath)
    {
        //BINARY READER EXAMPLE -> BT GENERATES 112 CHANNELS FOR SOME REASON Oo
        int c = 0;

        FileStream fs = new FileStream("C:/BioTrace/System/DataChan.bin", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        //FileStream fs = new FileStream("C:/Users/V/Desktop/test.bin", FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs, Encoding.ASCII);

        double x;
        int i = 0;
        while (true)
        {
            try
            {
                x = br.ReadDouble();
                buffer[i] = x;
                if (i == 2 || i == 3 || (i < 22 && i > 9))
                    Console.WriteLine("Pos: " + i + " Val: " + x);

                i++;
            }
            catch (EndOfStreamException e)
            {
                //Buffer full (1 sample)
                fs.Seek(0, SeekOrigin.Begin);
                i = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            };
        }
    }
}

