using DevTools.MVVM.Model;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using DevTools.MVVM.Model.DTO;

namespace DevTools.Services.ConfigurationFile
{
    public class ConfigurationFileBuilder : IConfigurationFileBuilder
    {
        /// <summary>
        /// Permette di creare un file .cfg nella cartella di versione segnalata, e di decidere se eventualmente 
        /// creare anche i record indispensabili
        /// </summary>
        /// <param name="devToolModel">Nome dell'ARCPROC da usare</param>
        public void CreaCfg(DevToolModel devToolModel)
        {
            try
            {
                //Preparo le variabili
                string strConnection = devToolModel.CfgConfiguration!.GetArcprocConnetionString();
                string strFileName = devToolModel.CfgConfiguration!.GetCfgFileName();
                //Se ha / o \ vuol dire che viene dalla parte Rive
                string strPercorso = GetFolderForCfg(devToolModel);
                String[] strComando = { $"{strConnection};LANGUAGE=us_english;APP=Business;Encrypt=True;TrustServerCertificate=True" };

                //Se esiste già il file lo cancello e lo scivo da capo
                if (File.Exists(strPercorso + $@"\{strFileName}.cfg"))
                {
                    File.Delete(strPercorso + $@"\{strFileName}.cfg");
                    File.WriteAllLines(strPercorso + $@"\{strFileName}.cfg", strComando, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllLines(strPercorso + $@"\{strFileName}.cfg", strComando, Encoding.UTF8);
                }
                Clipboard.SetText(strFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void SetUpConnection(DevToolModel devToolModel)
        {
            string strConnection = devToolModel.CfgConfiguration!.GetArcprocConnetionString();
            string strFileName = devToolModel.CfgConfiguration!.GetCfgFileName();
            string strPercorso = GetFolderForCfg(devToolModel);
            //Creo le connessioni necessarie al DB
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = strConnection;
            sqlConnection.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
            //Inizio a scrivere quello che mi serve
            String strInsert = "INSERT INTO regedit (re_pc, re_user, re_software, re_profil, re_detail, re_nomprop, re_valprop)";
            //SystemInst
            try
            {
                cmd.CommandText = strInsert + $@" VALUES('{System.Environment.MachineName}', '*', 'Business', '*', '*', 'SystemInst', 1)";
                cmd.ExecuteNonQuery();
            }
            catch { }
            //Dir
            try
            {
                cmd.CommandText = strInsert + $@" VALUES('{System.Environment.MachineName}', '*', 'Business', '{strFileName}', '*', 'Dir', '{strPercorso}')";
                cmd.ExecuteNonQuery();
            }
            catch { }
            //OfficeDir
            try
            {
                cmd.CommandText = strInsert + $@" VALUES('{System.Environment.MachineName}', '*', 'Business', '{strFileName}', '*', 'OfficeDir', '{strPercorso}\Office')";
                cmd.ExecuteNonQuery();
            }
            catch { }
            //RptDir
            try
            {
                cmd.CommandText = strInsert + $@" VALUES('{System.Environment.MachineName}', '*', 'Business', '{strFileName}', '*', 'RptDir', '{strPercorso}\Rpt')";
                cmd.ExecuteNonQuery();
            }
            catch { }
            //BusAggDir
            try
            {
                cmd.CommandText = strInsert + $@" VALUES('{System.Environment.MachineName}', '*', 'Business', '{strFileName}', 'BusAgg', 'BusAggDir', '{strPercorso}\Agg')";
                cmd.ExecuteNonQuery();
            }
            catch { }
            //AggNumber
            try
            {
                cmd.CommandText = strInsert + $@" VALUES('{System.Environment.MachineName}', '*', 'Business', '{strFileName}', 'BusAgg', 'BusAggDir', '2300')";
                cmd.ExecuteNonQuery();
            }
            catch { }
            //SystemInst
            try
            {
                cmd.CommandText = strInsert + $@" VALUES('{System.Environment.MachineName}', '*', 'Business', '{strFileName}', 'BusAgg', 'BusAggAutoUpdate', 'N')";
                cmd.ExecuteNonQuery();
            }
            catch { }
            //Server
            try
            {
                cmd.CommandText = strInsert + $@" VALUES('{System.Environment.MachineName}', '*', 'Business', '{strFileName}', '*', 'Server', 'N')";
                cmd.ExecuteNonQuery();
            }
            catch { }
            //Key
            try
            {
                DataTable dttProfilo = new DataTable();
                cmd.CommandText = "SELECT TOP 1 re_pc, re_profil FROM regedit WHERE re_detail = 'ActKey' AND re_nomprop = 'AltreOpzioni' ORDER BY re_valprop DESC";
                dttProfilo.Load(cmd.ExecuteReader());
                if (dttProfilo.Rows.Count > 0)
                {
                    cmd.CommandText = strInsert + $@" SELECT '{System.Environment.MachineName}', '*', 'Business', '{strFileName}',re_detail, re_nomprop, re_valprop
                                            FROM regedit WHERE re_pc = '{dttProfilo.Rows[0]["re_pc"].ToString()}'
                                            AND re_profil = '{dttProfilo.Rows[0]["re_profil"].ToString()}'
                                            AND re_detail = 'ActKey'";
                    cmd.ExecuteNonQuery();
                }

            }
            catch { }
            //Elimina il GDPR
            if (devToolModel.DeleteGdpr)
            {
                try
                {
                    cmd.CommandText = $"UPDATE GDPRCONF SET gd_attivagdpr = 'N', gd_gestscadpwd = 'N'";
                    cmd.ExecuteNonQuery();
                }
                catch { }
                try
                {
                    cmd.CommandText = $"INSERT INTO OPERAT (OpNome, OpPasswd)" +
                                        $"VALUES ('Admin', '')";
                    cmd.ExecuteNonQuery();
                }
                catch { }
                try
                {
                    cmd.CommandText = $"UPDATE OPERAT SET OpPasswd = '' WHERE OpNome = 'Admin'";
                    cmd.ExecuteNonQuery();
                }
                catch { }
            }
            sqlConnection.Close();

            //Import della chiave
            String strKeyName = "AK3.key";
            if (devToolModel.BusinessCube!.Version.ToString().ToUpper() != String.Empty ||
                devToolModel.BusinessCube!.Version.ToString().ToUpper() != "SR7")
            {
                strKeyName = "AK2.key";
            }
            else
            {
                strKeyName = "AK3.key";
            }
            if (File.Exists($@"\\vss\SRCSAFE\DevTools\BusKey\{strKeyName}"))
            {
                ImportaKey(strKeyName, strConnection, strFileName);
            }
        }
        private string GetFolderForCfg(DevToolModel devToolModel)
        {
            if (!string.IsNullOrEmpty(devToolModel.BusinessCube?.Folder))
            {
                return devToolModel.BusinessCube?.Folder!;
            }
            else
            {
                if (devToolModel.isExpirience() && devToolModel.isExpirienceRTM()) 
                { 
                    return $@"C:\{devToolModel.BusinessCube!.Version.ToString()}\TEST"; 
                }
                else 
                { 
                    return $@"C:\biz2017{devToolModel.BusinessCube!.Version.ToString()}\TEST"; 
                }
            }
        }
        private void ImportaKey(String strFileName, string strConnection, string strProfilo)
        {
            String strNomprop = String.Empty;
            String strValprop = String.Empty;
            String strProductType = String.Empty;
            String strTypo = String.Empty;
            String strActKey = "ActKey";
            SqlTransaction traDb;
            decimal nRes = 0;
            try
            {
                if (!File.Exists(@"\\vss\SRCSAFE\DevTools\BusKey\" + strFileName))
                {
                    return;
                }
                StreamReader r1 = new StreamReader(@"\\vss\SRCSAFE\DevTools\BusKey\" + strFileName, System.Text.Encoding.UTF8);
                String strKeyFile = r1.ReadToEnd();
                r1.Close();
                //Creo le connessioni necessarie al DB
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = strConnection;
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConnection;
                //prendo il nome della macchina
                String strWhere = System.Environment.MachineName;
                String[] strW = new String[] { strWhere };
                //dichiaro l'inizio transazione
                traDb = sqlConnection.BeginTransaction();

                //Inizio a ciclare gli elementi della chiave
                Dictionary<String, String> dicKey = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
                foreach (String strT in strKeyFile.Replace(System.Environment.NewLine, "§").Split('§'))
                {
                    if ((strT.Trim() != "") && (!strT.StartsWith("[KeyVersion")))
                    {
                        strNomprop = strT.Split('=')[0].Trim();
                        strValprop = strT.Split('=')[1].Trim();

                        strNomprop = strNomprop.Substring(1, strNomprop.Length - 2);
                        strValprop = strValprop.Substring(1, strValprop.Length - 2);

                        strNomprop = strNomprop.Replace(@"""""", @"""").Trim();
                        strValprop = strValprop.Replace(@"""""", @"""").Trim();

                        if (dicKey.ContainsKey(strNomprop))
                        {
                            MessageBox.Show("Nel file .KEY è stata ripetuta più volte la chiave '" + strNomprop + "'." + System.Environment.NewLine + "Impossibile proseguire con l'importazione.");
                            return;
                        }

                        dicKey.Add(strNomprop, strValprop);
                    }
                }
                strTypo = dicKey["ModuliExt"].ToUpper();
                if ((strTypo[10].ToString() == "S") || (strTypo[11].ToString() == "S") || (strTypo[12].ToString() == "S"))
                {
                    strProductType = "E"; //Easy
                }
                else
                {
                    strTypo = dicKey["ModuliSup"].ToUpper();
                    if (strTypo[29].ToString() == "S")
                    {
                        strProductType = "F"; //Friendly
                    }
                }

                if (!dicKey.ContainsKey("ProductType"))
                {
                    dicKey.Add("ProductType", strProductType);
                }
                else
                {
                    dicKey["ProductType"] = strProductType;
                }
                if (!CheckActKey(dicKey))
                {
                    return;
                }
                foreach (String strKey in dicKey.Keys)
                {
                    for (int i = 0; i < strW.Length; i++)
                    {
                        cmd.CommandText = $"UPDATE regedit SET re_valprop = '{dicKey[strKey]}'" +
                                          $" WHERE re_pc = '{strW[i].Split('§')[0].Trim()}'" +
                                          $" AND re_user = '*'" +
                                          $" AND re_nomprop = '{strKey}'" +
                                          $" AND re_detail = '{strActKey}'" +
                                          $" AND re_software = 'Business'" +
                                          $" AND re_profil = '{strProfilo}'";
                        cmd.Transaction = traDb;
                        nRes = cmd.ExecuteNonQuery();
                        if (nRes == 0)
                        {
                            //nuova proprietà: la aggiungo
                            cmd.CommandText = $"INSERT INTO regedit (re_pc, re_user, re_software, re_profil, re_detail, re_nomprop, re_valprop) " +
                                              $"VALUES ( '{strW[i].Split('§')[0].Trim()}', " +
                                              $" '*', " +
                                              $"'Business', " + "'" + (strProfilo) + "', " +
                                              "'" + strActKey + "', " +
                                              "'" + strKey + "', " +
                                              "'" + dicKey[strKey] + "')";
                            cmd.ExecuteNonQuery();
                        }
                        if (!dicKey.ContainsKey("ModuliSup2"))
                        {
                            cmd.CommandText = "DELETE FROM regedit " +
                                              " WHERE re_pc = '" + strW[i].Split('§')[0].Trim() + "'" +
                                              " AND re_user = '*'" +
                                              " AND re_detail = '" + strActKey + "'" +
                                              " AND re_software = 'Business'" +
                                              " AND re_profil = '" + strProfilo + "'" +
                                              " AND re_nomprop IN ('ModuliSup2', 'ModuliSup2Ext')";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                traDb.Commit();
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private bool CheckActKey(Dictionary<String, String> dicKey)
        {
            String strModuliSup2 = String.Empty;
            String strModuliSup2Ext = String.Empty;
            StringBuilder strDtscad = new StringBuilder();
            int.TryParse(Assembly.GetEntryAssembly()?.GetName().Version?.ToString().Split('.')[0], out int lRelMajor);
            int.TryParse(dicKey["Matricola"], out int lMatricola);
            String strNomlic = dicKey["NomeLic"];
            String strParivalic = dicKey["PartitaIva"];
            String strAltreopz = dicKey["AltreOpzioni"];
            String strActkeyReg = dicKey["Actkey"];
            String strModuli = dicKey["Moduli"];
            String strModuliExt = dicKey["ModuliExt"];
            String strModuliSup = dicKey["ModuliSup"];
            String strModuliSupExt = dicKey["ModuliSupExt"];
            String strModuliPtn = dicKey["ModuliPtn"];
            String strModuliPtnExt = dicKey["ModuliPtnExt"];
            String strMultiKey = dicKey["Multikey"];
            String strTipoKey = dicKey["TipoKey"];
            Dictionary<String, String> dicKey2 = dicKey;
            //Boolean bOk = false;
            if (dicKey.ContainsKey("ModuliSup2"))
            {
                strModuliSup2 = dicKey["ModuliSup2"];
                strModuliSup2Ext = dicKey["ModuliSup2Ext"];
            }
            //If strCheckType = "C" Then
            //usa la nuova BN__CHAK in c++. la BN__CHAK.DLL non va registrata e deve essere nella dir di BE__MENU.DLL
            strDtscad.Capacity = 512;
            strDtscad.Append("".PadLeft(100));
            //Il modulo friendly deve essere disabilitato, altrimenti la chiave non è valida
            if (!strModuliSup.EndsWith("N"))
            {
                MessageBox.Show("Non sono ammesse chiavi di Friendly su installazioni Business.");
                return false;
            }
            return true;
        }
        private bool IsWin64Bit()
        {
            return IntPtr.Size != 4;
        }

    }
}
