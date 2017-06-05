using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Recommended_musical_scale_using_MIDI
{
    /// <summary>
    /// SerialPortConnect.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SerialPortConnect : Window
    {
        private SerialPort _Port;
        private SerialPort Port
        {
            get
            {
                if (_Port == null)
                {
                    _Port = new SerialPort();
                    _Port.PortName = "COM1";
                    _Port.BaudRate = 9600;
                    _Port.DataBits = 8;
                    _Port.Parity = Parity.None;
                    _Port.Handshake = Handshake.None;
                    _Port.StopBits = StopBits.One;
                    _Port.Encoding = Encoding.UTF8;
                    _Port.DataReceived += Port_DataReceived;
                }
                return _Port;
            }
        }
        private StringBuilder _Strings;
        /// <summary>
        /// 로그 객체
        /// </summary>
        private String Strings
        {
            set
            {
                if (_Strings == null)
                    _Strings = new StringBuilder(1024);
                // 로그 길이가 1024자가 되면 이전 로그 삭제
                if (_Strings.Length >= (1024 - value.Length))
                    _Strings.Clear();
                // 로그 추가 및 화면 표시
                _Strings.AppendLine(value);
                TB_DATA.Text = _Strings.ToString();
            }
        }
        /// <summary>
        /// SerialPort : Data Received Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String msg = Port.ReadExisting();
            TB_DATA.Text+= msg;
        }
        private Boolean IsOpen
        {
            get { return Port.IsOpen; }
            set
            {
                if (value)
                {
                    Strings = "연결 됨";
                    BT_SerialConnect.Content = "연결 끊기";
                
                  
                }
                else
                {
                    Strings = "연결 해제됨";
                    BT_SerialConnect.Content = "연결";
                
                }
            }
        }
        private void SerialComm_Load()
        {
            // 시리얼포트 목록 갱신
            foreach(string s in SerialPort.GetPortNames())
            {
                CB_Port.Items.Add(s);
            }

            CB_Baudrate.Items.Add(115200);
            CB_Baudrate.Items.Add(19200);
            CB_Baudrate.Items.Add(38400);
            CB_Baudrate.Items.Add(57600);
            CB_Baudrate.Items.Add(9600);

            CB_Data_Size.Items.Add(8);
            CB_Data_Size.Items.Add(7);
            CB_Data_Size.Items.Add(6);

           CB_Parity.Items.Add("none");
           CB_Parity.Items.Add("even");
           CB_Parity.Items.Add("mark");
           CB_Parity.Items.Add("odd");
           CB_Parity.Items.Add("space");

            CB_HandSake.Items.Add("none");
            CB_HandSake.Items.Add("Xon / Xoff");
            CB_HandSake.Items.Add("request to send");
            CB_HandSake.Items.Add("request to send Xon / Xoff");

            // 기타 셋팅 목록 기본값 선택
            CB_Baudrate.SelectedIndex = 0;
            CB_Data_Size.SelectedIndex = 0;
            CB_Parity.SelectedIndex = 0;
            CB_HandSake.SelectedIndex = 0;
            
        }

        public SerialPortConnect()
        {
            InitializeComponent();
            SerialComm_Load();
        }

        private void BT_SerialConnect_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!Port.IsOpen)
                {
                    // 현재 시리얼이 연결된 상태가 아니면 연결.
                    Port.PortName = CB_Port.SelectedItem.ToString();
                    Port.BaudRate = Convert.ToInt32(CB_Baudrate.SelectedItem);
                    Port.DataBits = Convert.ToInt32(CB_Data_Size.SelectedItem);
                    Port.Parity = (Parity)CB_Parity.SelectedIndex;
                    Port.Handshake = (Handshake)CB_HandSake.SelectedIndex;

                    try
                    {
                        // 연결
                        Port.Open();
                    }
                    catch(Exception ex)
                    {
                        Strings = String.Format("[ERR] {0}", ex.Message);
                    }
                }
                else
                {
                    // 현재 시리얼이 연결 상태이면 연결 해제
                    Port.Close();
                }

                // 상태 변경
                IsOpen = Port.IsOpen;
            }
            catch(Exception ex)
            {
                MessageBox.Show("잘못된 포트입니다."+ex.Message);
            }
        }

        private void BT_Sender_Click(object sender, RoutedEventArgs e)
        {
             // 보낼 메시지가 없으면 종료
            String text = TB_DATA.Text.Trim();
            if (String.IsNullOrEmpty(text)) return;

            try
            {
                // 메시지 전송
                Port.WriteLine(text);
                // 표시
                Strings = String.Format("[SEND] {0}", text);
            }
            catch (Exception ex) { Strings = String.Format("[ERR] {0}", ex.Message); }
        }
        
    }
}
