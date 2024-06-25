using System;
using System.Collections.Generic;

namespace ExperimentScripts.Model
{
    public class ExperimentData
    {
        public string Modus { get; set; }
        public string ExpId { get; set; }
        public int Round { get; set; }
        public string Text { get; set; }
        public float WordsPerMinute { get; set; }
        public float ErrorRate { get; set; }
        public List<string> TypedKeys { get; set; }
        public List<Keystroke> Keystrokes { get; set; }
        
        
    }

    public class Keystroke
    {
        public float Time { get; set; }
        public string Key { get; set; }
        public string Hand { get; set; }
        public string Finger { get; set; }
    }
}