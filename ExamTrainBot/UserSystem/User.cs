using ExamTrainBot.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamTrainBot
{
    [Serializable]
    public class User
    {
        public long id;
        public string name;
        public bool subscriber,isadmin, ontest;
        public int points, currentquestion;
        public bool testcreation;

        public static int currenttest = 0;

        public User(long id, string name, bool ispayeduser, bool isadmin)
        {
            this.id = id;
            this.name = name;
            this.subscriber = ispayeduser;
            this.isadmin = isadmin;
            ontest = false;
            points = 0;
            currentquestion = 0;
            currenttest = 0;
            testcreation = false;
        }
    }
}
