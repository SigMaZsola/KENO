using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kenó
{
    internal class NapiKeno
    {
        int ev;

        private int _het;
        public bool hibas { get; private set; }
        public int het
        {
            get { return _het; }
            set
            {
                if (value < 1) value = 1;
                if (value > 52) value = 52;
                _het = value;
            }
        }

        private int _nap;
        public int nap
        {
            get { return _nap; }
            set
            {
                if (value < 1) value = 1;
                if (value > 7) value = 7;
                _nap = value;
            }
        }

        string huzasDatum;
        public List<int> huzottSzamok { get; private set; } = new List<int>();

        public NapiKeno(string beolvasottSor)
        {
            string[] adat = beolvasottSor.Split(';');

            ev = Convert.ToInt32(adat[0]);
            het = Convert.ToInt32(adat[1]);
            nap = Convert.ToInt32(adat[2]);

            huzasDatum = adat[0] + "." + adat[1] + "." + adat[2];

            for (int i = 4; i < adat.Length; i++)
            {
                huzottSzamok.Add(Convert.ToInt32(adat[i]));
            }

            if (huzottSzamok.Count != 20)
            {
                hibas = true;

      
                huzottSzamok.Clear();
                for (int i = 0; i < 20; i++)
                {
                    huzottSzamok.Add(0);
                }
            }
        }
        public int TalalatSzam(List<int> tippek)
        {
            int talalatok = 0;
            foreach (var item in tippek)
            {
                if (huzottSzamok.Contains(item))
                {
                    talalatok++;
                }
            }
            return talalatok;
        }
        public bool Helyes()
        {
            return huzottSzamok.Count() == 20 && huzottSzamok.Distinct().Count() == 20;

                
        }
    }
    
}
