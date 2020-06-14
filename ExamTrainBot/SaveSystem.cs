using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using ExamTrainBot;
using ExamTrainBot.Commands;
using Json.Net;
using ExamTrainBot.Tests;
using Newtonsoft.Json;

namespace ExamTrainBot
{
    public static class SaveSystem
    {
        public static void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/users.sv";
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, Program.users);
            stream.Close();
        }
        public static void SaveTests()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/tests.sv";
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, Program.testlist);
            stream.Close();
        }
        public static void Load()
        {
            //Load users
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/users.sv";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                Program.users = formatter.Deserialize(stream) as List<User>;

                stream.Close();
            }
            //Load Tests
            path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/tests.sv";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                Program.testlist = formatter.Deserialize(stream) as List<Test>;

                stream.Close();
            }
        }
    }
}