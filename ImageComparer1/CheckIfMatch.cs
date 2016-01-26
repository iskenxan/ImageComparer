using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageComparer1
{
    class CheckIfMatch
    {
        UpdateProgressDelegate UpdateProgress;//Updates ProgressBar when called
        public CheckIfMatch(UpdateProgressDelegate UpdateProgresBar)
        {
            UpdateProgress = UpdateProgresBar;
        }
        public bool CompareStrings(List<string> subSet, List<string> superSet)
        {//compares two Lists of strings to see if the superSet list of strings , contains more than 90 percent of subset list of strings
            bool match = false;
            List<string> string1 = superSet;
            List<string> string2 = subSet;
            int similar=0 , different=0;
            bool contains = false;
            double percentage;
            int i = 0;
            foreach (string small in string2)
            {
                foreach (string large in string1)
                {
                    contains = false;
                    if (large.Contains(small))
                    {
                        similar++;
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                    different++;
                i++;
                if(i%20==0)//Updates programm only after each tenth iteration, this makes program run a bit faster
                UpdateProgress();
            }
            percentage = (double)similar / (similar + different);

            if (percentage >= 0.9)
            {
                match = true;
            }

            return match;
        }
    }
}
