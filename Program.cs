using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

namespace DeployCertUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string certHash = "";
                string certStore = "MY";

                X509Store certificateStore = new X509Store(StoreLocation.LocalMachine);
                certificateStore.Open(OpenFlags.ReadOnly);
        
                foreach (var certificate in certificateStore.Certificates)
                {
                    bool isServerAuth = false;
                    Console.WriteLine("[Info] " + certificate.GetCertHashString());
                    foreach (X509Extension extension in certificate.Extensions)
                    {
                        Console.WriteLine(extension.Format(true));
                        Console.WriteLine(extension.Oid.Value.ToString());
                        if (extension.Format(true).Contains("Server Authentication"))
                        {
                            isServerAuth = true;
                        }
                        if (certificate.Verify() && isServerAuth)
                        {
                            certHash = certificate.GetCertHashString();
                        }
                        else if (certificate.Verify() && isServerAuth)
                        {
                            certHash = certificate.GetCertHashString();
                        }
                        else if (certificate.Verify())
                        {
                            certHash = certificate.GetCertHashString();
                        }

                    }
                }
                certificateStore.Close();
                ServerManager mgr = null;

                string server = null; // or remote machine name
                string siteName = "Default Web Site";

                string bindingProtocol = "https";
                string bindingInfo = "*:7443:";


                if (String.IsNullOrEmpty(server))
                {
                    mgr = new ServerManager();
                }
                else
                {
                    mgr = ServerManager.OpenRemote(server);
                }

                Site site = mgr.Sites[siteName];

                Binding binding = null;
                foreach (Binding b in site.Bindings)
                {
                    Console.WriteLine(bindingInfo);
                    if (b.Protocol.Equals(bindingProtocol)
                        && b.BindingInformation.Equals(bindingInfo))
                    {
                        binding = b;
                        break;
                    }
                }

                if (binding == null)
                {
                    throw new Exception("Binding not found!");
                }


                if (!String.IsNullOrEmpty(certHash))
                {
                    ConfigurationMethod method = binding.Methods["AddSslCertificate"];
                    if (method == null)
                    {
                        throw new Exception("Unable to access the AddSslCertificate configuration method");
                    }

                    ConfigurationMethodInstance mi = method.CreateInstance();
                    mi.Input.SetAttributeValue("certificateHash", certHash);
                    mi.Input.SetAttributeValue("certificateStoreName", certStore);
                    mi.Execute();

                    Console.WriteLine("Certificate has been added: " + certHash);
                }
                else
                {
                    Console.WriteLine("Certificate can not be found");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                Console.Read();
                Environment.Exit(-1);
            }
            Thread.Sleep(9000);
        }
    }
}
