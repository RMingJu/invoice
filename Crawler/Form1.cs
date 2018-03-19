using System;
using System.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Crawler
{
    public partial class Form1 : Form
    {
        List<String> now = null;
        List<String> past = null;
        StringBuilder nowString = null;
        StringBuilder pastString = null;
        String prizeMsg="";
        SoundPlayer WinMusic = new SoundPlayer(Crawler.Properties.Resources.H_Do);
        public Form1()
        {
            InitializeComponent();
            label1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            now = new List<string>();
            past = new List<string>();
            nowString = new StringBuilder();
            pastString = new StringBuilder();

            WebClient web = new WebClient();
            StringBuilder sb = new StringBuilder();
            StringBuilder htmltag = new StringBuilder();
            StringBuilder rgxTemp = new StringBuilder();
            List<String> temp = new List<string>();


            //讀取網頁的原始檔
            Stream web_stream = web.OpenRead("http://invoice.etax.nat.gov.tw");
            //讀取字並轉碼
            StreamReader sr = new StreamReader(web_stream, Encoding.UTF8);
            while (!sr.EndOfStream)
            {
                sb.Append(sr.ReadLine()).Append("\r\n");
            }
            //關閉串流
            sr.Dispose();
            web_stream.Dispose();
            web_stream.Close();
            sr.Close();


            Regex regex;
            //regex = new Regex(@"<h2>\w*.?\w*</h2;
            //regex = new Regex(@"<h2>\w*.?\w*</h2>");
            //regex = new Regex(@"</?[a-z][a-z0-9]*[^<>]*>");
            //regex = new Regex(@"<span [^<>]*>\w*\d*</span>");

            //取的時間放入combobox
            regex = new Regex(@"<h2>\d+年\d+[-]+\d+\w+");
            foreach (var ans in regex.Matches(sb.ToString()))
            {
                String ss = ans.ToString().Substring(4);
                comboBox1.Items.Add(ss);
            }


            regex = new Regex(@"<td><span [^<>]*>\s*\w*\s*[^<br>]\w*[^<br>]\w*[</span>]*[</td>]*");
            //抓出中獎號碼標籤
            foreach (var ans in regex.Matches(sb.ToString()))
            {
                
                htmltag.Append(ans).Append("\r\n");
            }


            //抓出中獎號碼
            regex = new Regex(@">\d+、*\d*、*\d*");
            

            foreach (var x in regex.Matches(htmltag.ToString()))
            {
                
                String s = x.ToString().Substring(1, x.ToString().Length-1);
                temp.Add(s);
                rgxTemp.Append(s).Append("\r\n");
            }

            //前半部為最新的後半部為前兩個月的中獎號碼
            for (int i = 0; i < temp.Count; i++)
            {
                if (i <= (temp.Count / 2) - 1)
                {
                    if (i == 2 || i == 3)
                    {
                       String[] s= temp[i].ToString().Split('、');
                        foreach (var x in s)
                        {
                            now.Add(x);
                        }
                    }
                    else
                    {
                        now.Add(temp[i]);
                    }

                 //now顯示字串
                        String ss = null;
                        switch (i)
                        {
                            case 0:
                                ss = String.Format("特別獎:{0}", temp[i]);
                                break;
                            case 1:
                                ss = String.Format("特獎:{0}", temp[i]);
                                break;
                            case 2:
                                ss = String.Format("頭獎:{0}", temp[i]);
                                break;
                            case 3:
                                ss = String.Format("增開六獎:{0}", temp[i]);
                                break;
                            default:
                                break;
                        }
                        nowString.Append(ss).Append("\r\n\r\n");
                    
                }
                else
                {
                    if (i == 6 || i == 7)
                    {
                        String[] s = temp[i].ToString().Split('、');
                        foreach (var x in s)
                        {
                            past.Add(x);
                        }
                    }
                    else
                    {
                        past.Add(temp[i]);
                    }


                    //past顯示字串
                    String ss = null;
                    switch (i)
                    {
                        case 4:
                            ss = String.Format("特別獎:{0}", temp[i]);
                            break;
                        case 5:
                            ss = String.Format("特獎:{0}", temp[i]);
                            break;
                        case 6:
                            ss = String.Format("頭獎:{0}", temp[i]);
                            break;
                        case 7:
                            ss = String.Format("增開六獎:{0}", temp[i]);
                            break;
                        default:
                            break;
                    }
                    pastString.Append(ss).Append("\r\n\r\n");

                }
            }
            comboBox1.SelectedIndex = 0;
            textBox2.Focus();
            //textBox1.Text = rgxTemp.ToString();
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = sender as ComboBox;
            switch (box.SelectedIndex)
            {
                case 0:
                    textBox1.Text = nowString.ToString();
                    break;
                case 1:
                    textBox1.Text = pastString.ToString();
                    break;
            }
            
        }


        private bool Prize(String barcode ,List<String> prize)
        {
            
            if (barcode.Length < 3)
            {
                MessageBox.Show("輸入錯誤");
                return false;
            }
            


            //擷取末三碼
            String lastThree = barcode.Substring(textBox2.Text.Length - 3);

            //對末三碼
            var x = from data in prize
                    where data.EndsWith(lastThree)
                    select data;
            if (x.Count() >= 1)
            {
                foreach (var ans in x.ToList())
                {
                    if (ans == prize[0] || ans == prize[1]) //特別獎 特獎 頭獎
                    {
                        prizeMsg = "中大獎?，趕快檢查!";
                        
                    }
                    else //普通2~6獎
                    {
                        prizeMsg = "中二~六獎!";
                    }
                }
                //MessageBox.Show(prizeMsg);
                return true;
               
            }
            else
            {
                return false;
            }

            
            


        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = sender as TextBox;
            List<String> prize = null;
            if (e.KeyCode == Keys.Enter)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        prize = now;
                        break;
                    case 1:
                        prize = past;
                        break;
                }
                if (Prize(box.Text, prize))
                {
                    label1.Text = prizeMsg;
                    label1.ForeColor = Color.Red;
                    WinMusic.Play();//中獎提示音
                }
                else
                {
                    label1.Text = "未中獎!";
                    label1.ForeColor = Color.Black;
                }

                textBox2.SelectAll();
            }
        }
    }
}
