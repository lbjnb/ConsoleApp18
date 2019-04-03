using System;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace ConsoleApp18
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("你啥都没输入想生成什么?");
            }

            else if (args.Length > 100)
            {
                Console.WriteLine("字数太多会被杀掉哦！");
            }
            else if (args.Length <= 1)
            {
                Console.WriteLine(" 我不想当单身狗");
            }
            else

            {
                int num = 0;
                
                for (int i = 0; i < args.Length; i++)
                {
                    for (int j = 0; j < args[i].Length; j++)
                    {
                        
                        if (!(((int)args[i][j] >= 65 && (int)args[i][j] <= 90) ||
                            ((int)args[i][j] >= 97 && (int)args[i][j] <= 122)))
                        {
                            Console.WriteLine("请输入英文");
                            num++;
                            break;
                        }
                    }
                    if (num == 1)
                    {
                        break;
                    }
                }
                if (num == 0)
                {
                   
                    string text = args[0];
                    for (int i = 1; i < args.Length; i++)
                    {
                        text = text + " " + args[i];
                    }


                    QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                    QrCode qrCode = qrEncoder.Encode(text);
                    Console.BackgroundColor = ConsoleColor.White;

                    
                    for (int j = 0; j < qrCode.Matrix.Width; j++)
                    {
                        for (int i = 0; i < qrCode.Matrix.Width; i++)
                        {
                            if (qrCode.Matrix[i, j])
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.Write('█');
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write('█');
                            }
                        }
                        Console.WriteLine();
                    }
                    
                    Console.ResetColor();
                }
            }
        }

       

        
        public static void CreateQrCode_txt(string fileName, string[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                CreateQrCode_strToPng(fileName, str[i], i + 1);
            }
        }

       
        public static void CreateQrCode_strToPng(string fileName, string str, int image_num)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);//QrCode纠错等级
            QrCode qrCode = qrEncoder.Encode(str);//生成QrCode
            GraphicsRenderer graphicsRenderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
            //将二维码保存为png，保存至当前目录下，并输出提示消息
            MemoryStream ms = new MemoryStream();
            graphicsRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
            Image img = Image.FromStream(ms);
            string num = "00";
            num = num + image_num;
            img.Save(num.Substring(num.Length - 3, 3) + ".png");
            Console.WriteLine("已经生成图片" + num.Substring(num.Length - 3, 3));
        }
        public static MemoryStream GetQRCode(string content, string iconPath, int moduleSize = 9)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = qrEncoder.Encode(content);

            GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(moduleSize, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
            Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
            Graphics g = Graphics.FromImage(map);
            render.Draw(g, qrCode.Matrix);

            //追加Logo图片 ,注意控制Logo图片大小和二维码大小的比例
            //PS:追加的图片过大超过二维码的容错率会导致信息丢失,无法被识别
            Image img = Image.FromFile(iconPath);

            Point imgPoint = new Point((map.Width - img.Width) / 2, (map.Height - img.Height) / 2);
            g.DrawImage(img, imgPoint.X, imgPoint.Y, img.Width, img.Height);

            MemoryStream memoryStream = new MemoryStream();
            map.Save(memoryStream, ImageFormat.Jpeg);

            return memoryStream;

        }
    }
}