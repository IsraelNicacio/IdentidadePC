using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace IdentidadePC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            StringBuilder machine = new StringBuilder();

            machine.AppendFormat("cpu: {0}", cpuId());
            machine.AppendLine();
            machine.AppendFormat("bios: {0}", biosId());
            machine.AppendLine();
            machine.AppendFormat("hardDrive: {0}", diskId());
            machine.AppendLine();
            machine.AppendFormat("motherboard: {0}", baseId());
            machine.AppendLine();
            machine.AppendFormat("video: {0}", videoId());

            txtMachine.Text = machine.ToString();

            GetEnderecoMAC1();

            //string id = empacota(cpuId()
            //     + biosId()
            //     + diskId()
            //     + baseId()
            //     + videoId()
            //     + macId());
        }

        public static string GetEnderecoMAC1()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                String enderecoMAC = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    // retorna endereço MAC do primeiro cartão
                    if (enderecoMAC == String.Empty)
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        enderecoMAC = adapter.GetPhysicalAddress().ToString();
                    }
                }
                return enderecoMAC;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string identificador(string wmiClass, string wmiProperty)
        ///Retorna o identificador do hardware
        {
            string resultado = "";
            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {

                //pega somente o primeiro
                if (resultado == "")
                {
                    try
                    {
                        resultado = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }

            }
            return resultado;
        }

        private string cpuId()
        {
            //Usa o primeiro identificador da CPU na ordem de preferencia
            //Não pega todos os identificadores, pois demora muito tempo
            string retVal = identificador("Win32_Processor", "UniqueId");
            if (retVal == "") //Se não obter o UniqueID, usa o ProcessorID
            {
                retVal = identificador("Win32_Processor", "ProcessorId");

                if (retVal == "") //Se não pegar o ProcessorId, usa o Name
                {
                    retVal = identificador("Win32_Processor", "Name");


                    if (retVal == "") //Se não pegar o Name, usa o Manufacturer
                    {
                        retVal = identificador("Win32_Processor", "Manufacturer");
                    }
                    //Adiciona o clock speed por segurança
                    retVal += identificador("Win32_Processor", "MaxClockSpeed");
                }
            }

            return retVal;

        }

        private string biosId()
        //identifica a BIOS
        {
            return identificador("Win32_BIOS", "Manufacturer")
            + identificador("Win32_BIOS", "SMBIOSBIOSVersion")
            + identificador("Win32_BIOS", "IdentificationCode")
            + identificador("Win32_BIOS", "SerialNumber")
            + identificador("Win32_BIOS", "ReleaseDate")
            + identificador("Win32_BIOS", "Version");
        }

        private string diskId()
        //ID do principal disco rigido
        {
            return identificador("Win32_DiskDrive", "Model")
            + identificador("Win32_DiskDrive", "Manufacturer")
            + identificador("Win32_DiskDrive", "Signature")
            + identificador("Win32_DiskDrive", "TotalHeads");
        }

        private string baseId()
        //ID da Motherboard
        {
            return identificador("Win32_BaseBoard", "Model")
            + identificador("Win32_BaseBoard", "Manufacturer")
            + identificador("Win32_BaseBoard", "Name")
            + identificador("Win32_BaseBoard", "SerialNumber");
        }

        private string videoId()
        //ID do controlador de video primário
        {
            return identificador("Win32_VideoController", "DriverVersion")
            + identificador("Win32_VideoController", "Name");
        }

        private string macId()
        //ID da rede habilitada
        {
            return string.Empty; //identificador("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }

        private string empacota(string text)
        //Empacota a string para 8 digitos
        {
            string retVal;
            int x = 0;
            int y = 0;
            foreach (char n in text)
            {
                y++;
                x += (n * y);
            }

            retVal = x.ToString() + "00000000";
            return retVal.Substring(0, 8);
        }
    }
}
