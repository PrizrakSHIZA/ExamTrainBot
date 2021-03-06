﻿using ExamTrainBot.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ExamTrainBot
{
    [Serializable]
    public class User
    {
        public long id;
        public string name;
        public bool subscriber,isadmin, ontest = false;
        public int currentquestion = 0;
        public bool testcreation = false;
        public List<int> points = new List<int>();
        public List<Test> completedtests = new List<Test>();
        public List<bool[]> mistakes = new List<bool[]>();
        public DateTime date;

        public static int currenttest = 0;
        public int currentTest_serializable { get { return currenttest; } set { currenttest = value; } }

        public User(long id, string name)
        {
            this.id = id;
            this.name = name;
            this.subscriber = false;
            this.isadmin = false;
        }
        public User(long id, string name, bool subscriber, bool isadmin, string points, string tests, string mistakes, DateTime date)
        {
            this.id = id;
            this.name = name;
            this.subscriber = subscriber;
            this.isadmin = isadmin;
            if (points != "" || tests != "" || mistakes != "")
            {
                this.points = points.Replace(" ", "").Split(Program.delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();
                int[] temp = tests.Replace(" ", "").Split(Program.delimiterChars, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
                for (int i = 0; i < temp.Length; i++)
                {
                    completedtests.Add(Program.testlist[temp[i] - 1]);
                }
                this.mistakes = JsonSerializer.Deserialize<List<bool[]>>(mistakes);
            }
            this.date = date;
        }
    }
}
