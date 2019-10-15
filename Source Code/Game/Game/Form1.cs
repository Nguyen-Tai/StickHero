using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Game
{
    public partial class Form1 : Form
    {
    #region   Biến
        private int x_tower1; //tọa x độ tower 1
        private int x_tower2; // tọa độ x tower2
        private int tower1_width; //độ rộng tower1
        private int tower2_width; //độ rộng tower2
        private int y_tower; //độ cao tower 1 và 2 bằng nhau 
        private int x_human; //tọa độ x của human
        private int y_human; // tọa độ y của human
        private bool space;  // biến cho phép nhấn phím Space 
        private int score; // biến tính điểm
        private int distance; // biến khoảng cách giữa 2 tower
        private double heightBridge;  // biến lưu kết quả độ dài của cầu
        private bool HumanMove;  // biến lưu hành động human di chuyển
        private bool fail;     // biến lưu kết quả fail hay thành công
        private bool Move;    // biến lưu hành động di chuyển toàn bộ ( human, tower, bridge) khi đã qua cầu thành công
        private bool createBridge; // biến khởi tạo cầu (cầu là 1 line) 
        private bool bridgeFalling; // biến cầu đang rơi
        private int angle = 0;    // biến góc ( hỗ trợ cầu rơi) 
        private static SoundPlayer bridge = new SoundPlayer(Properties.Resources.stick_grow_loop); // các file âm thanh
        private static SoundPlayer kick = new SoundPlayer(Properties.Resources.kick); 
        private static SoundPlayer bridgeFall = new SoundPlayer(Properties.Resources.fall);
        private static SoundPlayer dead = new SoundPlayer(Properties.Resources.dead);
        private static SoundPlayer sscore = new SoundPlayer(Properties.Resources.score);
        private static SoundPlayer scoremid = new SoundPlayer(Properties.Resources.score_mid);
        Point p1; // tọa đồ p1 của cầu
        Point p2;  // tọa đồ p2 của cầu
        Random rd = new Random(); //biến random 
        #endregion

    #region Các hàm
        public Form1()
        {
            InitializeComponent();
        }
        private double HeightBridge(Point p1,Point p2)   // hàm tính độ dài của cầu
        {
          return  Math.Sqrt(Math.Pow((p1.X - p2.X),2) + Math.Pow((p1.Y-p2.Y),2));         
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            pictureBox1.Size = new Size(30, 30); //thiết lấp size của human;
            x_tower1 = 0; // vị trí Tower 1 sát biên trái
            score = 0;  //điểm =0
            space = true; // cho phép nhấn phím Space
            label2.Text = score.ToString(); // label hiện điểm
            tower1_width = 60; //cho độ rộng tower1 = 60
            x_tower2 = x_tower1+rd.Next(100,200); // random khoảng cách giữa tower1 và tower2
            tower2_width = rd.Next(40,70); // random độ rộng của tower2
            y_tower = 380; // cho độ cao của 2 tower =380
            x_human = x_tower1 + tower1_width - pictureBox1.Width-5 ; // cho tọa độ humman cách bìa trái 1 khoảng
            y_human = 350; 
            timer1.Interval = 10;
            p1 = new Point(x_tower1+tower1_width, y_tower-1); // cho tọa độ p1 của cầu tại rìa phải của cầu 
            p2 = p1; 

        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), x_tower1, y_tower, tower1_width, this.Height - y_tower); // vẽ tower1
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), x_tower2, y_tower, tower2_width, this.Height - y_tower);  // vễ tower2
                e.Graphics.FillRectangle(new SolidBrush(Color.Red),x_tower1+tower1_width/2 -4,y_tower,8,6);  // vẽ didemr chính giữa của tower1
                e.Graphics.FillRectangle(new SolidBrush(Color.Red),x_tower2+tower2_width/2-4, y_tower, 8, 6); // // vẽ didemr chính giữa của tower1
               e.Graphics.DrawLine(new Pen(Color.Black,3), p1,p2);  // vẽ cầu
                pictureBox1.Location = new Point(x_human, y_human);  // set tọa độ mới cho human
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (createBridge) // khi biến tạo cầu = true ( sau khi bấm space)
            {
                y_human = 350; // set lại chiều cao của human (cần thiết ở các bước sau) 
                p2.Y -= 4;     // tọa độ p2.y của cầu giảm  ( cầu đang cao lên) 
                this.Invalidate(); // vẽ lại
            }
            if (bridgeFalling) 
            {
                
                p2.X = Convert.ToInt32(p1.X + (heightBridge * Math.Sin((angle * Math.PI) / 180))); // (angle * Math.Pi)/180 để đổi góc sang radian
                p2.Y = Convert.ToInt32(p1.Y - (heightBridge * Math.Cos((angle * Math.PI) / 180)));
                angle += 5; 
                this.Invalidate(); 
                if (p1.Y == p2.Y && p1.X + heightBridge == p2.X) 
                {
                    bridgeFall.Play();
                    if (p2.X <= x_tower2 || p2.X >= x_tower2 + tower2_width) 
                       
                    {
                        angle = 90;
                    }
                    else
                        angle = 0;
                    bridgeFalling = false; 
                    HumanMove = true; 
                }              
            }        
            if (HumanMove) 
            {
                if (p2.X <= x_tower2 || p2.X >= x_tower2 + tower2_width) //fail
                {
                    x_human += 5; 
                    if (x_human > p1.X + heightBridge) 
                    {
                        y_human = 350; 
                    }
                    else
                    {
                        y_human = 348; 
                                       
                    }
                    if (x_human>=p2.X) 
                    {
                        HumanMove = false;  // không cho lặp lại
                        fail = true; // qua cầu không thành công.
                        Thread.Sleep(50);
                        dead.Play();
                    }               
                }
                else if(p2.X>x_tower2+tower2_width/2-4 && p2.X<x_tower2+tower2_width/2+4) //+2 score khi  chạm vào điểm đỏ chính giữa tower
                {
                    x_human += 5;
                    if (x_human >= x_tower2 + tower2_width - pictureBox1.Width - 10)
                    {
                        HumanMove = false; // không lặp lại
                        score+=2; // +2 score
                        scoremid.Play();
                        Move = true; 
                    }
                    if (x_human > p1.X + heightBridge) 
                    {
                        y_human = 350;
                    }
                    else
                    {
                        y_human = 348;
                    }
                    label2.Text = score.ToString(); 
                }
                else  // +1 score khi không chạm vào điểm chính giữa
                {
                    x_human += 5;
                    if (x_human >= x_tower2 + tower2_width - pictureBox1.Width - 10) 
                    {
                        HumanMove = false;
                        score++;
                        sscore.Play();
                        Move = true;
                    }
                    if (x_human > p1.X + heightBridge) // y_haman
                    {
                        y_human = 350;
                    }
                    else
                    {
                        y_human = 348;
                    }
                    
                    label2.Text = score.ToString();
                }
                this.Invalidate(); //vẽ lại
            }
            if (fail) //khi bị fail
            {
                p2.X = Convert.ToInt32(p1.X + (heightBridge * Math.Sin((angle * Math.PI) / 180))); 
                p2.Y = Convert.ToInt32(p1.Y - (heightBridge * Math.Cos((angle * Math.PI) / 180)));
                angle += 5;
                y_human += 10;
                if(x_human+pictureBox1.Width>x_tower2)
                {
                    pictureBox1.Size = new Size( pictureBox1.Width-(x_human+pictureBox1.Width-x_tower2),30);
                }
                if(x_human>x_tower2+tower2_width)
                {
                    pictureBox1.Size = new Size(30, 30);

                }
                this.Invalidate(); //vẽ lại
                if (p1.X == p2.X && p1.Y + heightBridge == p2.Y) 
                {
                    angle = 0; 
                    fail = false; // khong lặp lại
                    timer1.Stop(); // dừng timer
                   DialogResult result= MessageBox.Show("Điểm của bạn: "+score.ToString()+"\n Bạn có muốn chơi lại", "Game Over", MessageBoxButtons.OKCancel);
                   if(result==DialogResult.OK)
                   {
                        Form1_Load(sender,e); //tọa ván chơi mới
                        this.Invalidate();
                   }
                   else
                   {
                        Application.Exit();
                   }
                }
            }
            if (Move)  // di chuyển toàn bộ màn hình ( khi đã qua cầu thành công) 
            {
                p1 = new Point(x_tower1 + tower1_width, y_tower - 1); // set lại tọa độ của cầu tại rìa phải tower2
                p2 = p1;
                x_tower2 -= 10; //di chuyển tower2
                x_tower1 -= 10; // di chyển tower1
                p1.X -= 10; // di chuyển cầu
                p2.X -= 10;
                x_human -= 10;  // di chyển human
                this.Invalidate(); // vẽ lại
                if (x_human-30<=0) //Human cách tường ( rìa phải của form) là 30
                {
                    x_tower1 += 10; 
                    p1.X += 10;
                    p2.X += 10;
                    x_human += 10;
                    if (x_tower2 - (x_tower1 + tower1_width)<=distance)  
                    {
                        Move = false; // không lăp lại
                        space = true; // cho phép nhấm space cho lầm tạo cầu tiếp theo 
                    }
                }
                if (x_tower1 + tower1_width <= 0) 
                  
                {
                    distance = rd.Next(70, 280);
                    x_tower1 = x_tower2; 
                    tower1_width = tower2_width; 
                    x_tower2 = this.Width; 
                    tower2_width = rd.Next(22, 60);  
                }
            } 
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) // nếu nhấm phím space
            {
                if(space)  // cho phép thao tác nhấn 
                {
                    bridge.PlayLooping(); // phát âm thanh tạo cầu lặp lại liên tục
                    createBridge = true;  // cho biến tạo cầu bằng true
                    space = false;       // không cho phép nhấm Space
                    timer1.Start();     // khởi động timer
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)  // thả phím Space
            {
                if (createBridge)  // thức thi nếu cầu đang được tạo
                {
                    bridge.Stop();  // dừng âm thanh tạo cầu
                    kick.Play();   // phát âm thanh
                    createBridge = false; // cho biến tao cầu = false
                    heightBridge = HeightBridge(p1, p2);  // tính chiều dài của cầu đã tạo được
                    bridgeFalling = true; // cho biến ngã cầu bằng true
                    Thread.Sleep(250); // sleep trong 250ms
                }
            }
        }
        #endregion
    }
}
