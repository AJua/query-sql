using System;

namespace query_sql
{
    public class Arguments
    {
        public Arguments(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-source")
                {
                    DataSource = args[i + 1];
                }

                if (args[i] == "-step")
                {
                    Step = args[i + 1];
                }

                if (args[i] == "-query" || args[i] == "-q")
                {
                    Query = args[i + 1];
                }

                if (args[i] == "-offset" || args[i] == "-o")
                {
                    OffSet = Convert.ToInt32(args[i + 1]);
                }

                if (args[i] == "-interval" || args[i] == "-i")
                {
                    Interval = args[i + 1];
                }

                if (args[i] == "-index" || args[i] == "-idx")
                {
                    Index = args[i + 1];
                }
            }
        }

        public string DataSource { get; set; }

        public string Step { get; set; }
        public string Query { get; set; }
        public int OffSet { get; set; }
        public string Interval { get; set; }
        public string Index { get; set; }
    }
}