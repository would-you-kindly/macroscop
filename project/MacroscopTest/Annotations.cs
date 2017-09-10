using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MacroscopTest
{
    class Annotations
    {
        public List<Area> areas;

        public Annotations()
        {
            areas = new List<Area>();
        }

        public void LoadAnnotations()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text Files (*.txt) | *.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        areas.Clear();
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] values = line.Split(',');
                            areas.Add(new Area(int.Parse(values[0]), int.Parse(values[1]), 
                                int.Parse(values[2]), int.Parse(values[3])));
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            string result = string.Empty;

            foreach (var item in areas)
            {
                result += item.ToString();
            }

            return result;
        }
    }
}
