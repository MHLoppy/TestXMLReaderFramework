/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Xml;

namespace TestXMLReaderFramework
{
    class Program
    {
        public static string playerProfile = "";
        public static int cbpName = 0;
        public static string userName = "";
        public static string currentUserXml = "";
        public static bool senseThePresenceOfXmlNearby = false;

        static void Main(string[] args)
        {
            // surely there's a more efficient way of setting this up than row upon row of try/catch blocks?

            CheckForFile();

            if (senseThePresenceOfXmlNearby == true)
            {
                try // Find Profile block
                {
                    AssignUserName();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error finding profile: {ex}");
                    Console.WriteLine($"Attempting to bypass HnZ curse by using substituted username...");

                    try
                    {
                        AssignUserNameEstonian();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"Sorry boss, shit's fucked up: {ex2}");
                        //Environment.Exit(0);
                    }
                    //Environment.Exit(0);
                }

                try // Read XML block
                {
                    FindProfile();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error finding profile: {ex}");
                    //Environment.Exit(0);
                }

                try // Read XML block
                {
                    ReadXml();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading XML: {ex}");
                    //Environment.Exit(0);
                }

                try // Check XML block
                {
                    CheckXml();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing XML: {ex}");
                    //Environment.Exit(0);
                }

                try // Write XML block
                {
                    if (cbpName == 0)
                    {
                        Console.WriteLine("Adding #ICON169 to last game name...");
                        WriteXml();
                    }
                    else
                    {
                        Console.WriteLine("Nothing to do...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating XML: {ex}");
                    //Environment.Exit(0);
                }
            }

            else if (senseThePresenceOfXmlNearby == false)
            {
                Console.WriteLine("Unable to acess current_user.xml");
            }

            Console.WriteLine();
            Console.WriteLine("Main function finished. Press ENTER to continue...");
            Console.ReadLine();

            ///*
            /// TODO
            ///
            /// [DONE] 1) detect if #ICON169 is already in the expected part of the .dat file, don't write to the file if it is
            /// 2) #ICON169 MUST be removed if CBP is unloaded, otherwise it will bait CBP players into joining unloaded lobbies
            ///
        }

        static void AssignUserName() //separated logic from FindProfile to more eloquently deal with HnZ curse
        {
            userName = Environment.UserName;
        }

        static void AssignUserNameEstonian() //same logic, but factors in ~Estonian~
        {
            userName = "Kasutaja"; //HnZcursed :HnZdead:
        }

        static void FindProfile() // logic to find current user + their .dat file
        {
            currentUserXml = @"C:\Users\" + userName + @"\AppData\Roaming\Microsoft Games\Rise of Nations\PlayerProfile\current_user.xml";

            XmlDocument doc = new XmlDocument();
            doc.Load(currentUserXml);
            string ronName = doc.SelectSingleNode("ROOT/CURRENT_USER/@name").Value;

            playerProfile = @"C:\Users\" + userName + @"\AppData\Roaming\Microsoft Games\Rise of Nations\PlayerProfile\" + ronName + ".dat";

            Console.WriteLine("Windows username: " + userName);
            Console.WriteLine("RoN username: " + ronName);
        }

        static void ReadXml() // reads the last game name (mostly as a building block for later functions + troubleshooting rather than to use itself)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(playerProfile);
            XmlNode xmlNode = doc.SelectSingleNode("ROOT/GAMESPY/LAST_GAME_NAME");
            string name = xmlNode.InnerText;

            Console.WriteLine("Last game name: " + name);
            Console.WriteLine("If this is correct, Press ENTER to continue...");
            Console.ReadLine();
        }

        static void CheckXml() // checks if #ICON169 is already present in last game name
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(playerProfile);
            XmlNode xmlNode = doc.SelectSingleNode("ROOT/GAMESPY/LAST_GAME_NAME");
            string lastGameName = xmlNode.InnerText;

            if (lastGameName.Contains("#ICON169") == true)
            {
                cbpName = 1;
                Console.WriteLine("Last game name already contains #ICON169. Press ENTER to continue...");
                Console.ReadLine();
                return;
            }
            else
            {
                cbpName = 0;
                Console.WriteLine("#ICON169 not in last game name. Continuing...");
            }

        }

        static void WriteXml() // updates last game name to prefix with #ICON169
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(playerProfile);
            XmlNode xmlNode = doc.SelectSingleNode("ROOT/GAMESPY/LAST_GAME_NAME");
            xmlNode.InnerText = "#ICON169 " + xmlNode.InnerText;
            doc.Save(playerProfile);

            Console.WriteLine("New game name: " + xmlNode.InnerText);
        }

        static void CheckForFile()
        {
            userName = Environment.UserName;
            currentUserXml = @"C:\Users\" + userName + @"\AppData\Roaming\Microsoft Games\Rise of Nations\PlayerProfile\current_user.xml";

            if (File.Exists(currentUserXml))
            {
                senseThePresenceOfXmlNearby = true;
            }
            else
            {
                senseThePresenceOfXmlNearby = false; // this is surely not an efficient way of using a bool right? but I can't find an alternative in 3 minutes so it stays for now
            }
        }

    }
}
