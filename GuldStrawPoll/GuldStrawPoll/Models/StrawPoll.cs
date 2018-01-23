using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuldStrawPoll.Models
{
    public class StrawPoll
    {
        private bool multipleChoices;
        private String strawPollQuestion;
        private int nbrVotesStrawPoll;
        private String URLStrawPoll;
        private String URLSuppression;
        private String URLResults;

        public StrawPoll(String newStrawPollQuestion, bool newMultipleChoices, int newNbrVotes)
        {
            multipleChoices = newMultipleChoices;
            strawPollQuestion = newStrawPollQuestion;
            nbrVotesStrawPoll = newNbrVotes;
        }

        //METHODS//
        public String generateURLStrawPoll()
        {

            return "";
        }


        //GET & SET//
        public bool getMultipleChoices()
        {
            return multipleChoices;
        }

        public void setMultipleChoices(bool newMultipleChoices)
        {
            multipleChoices = newMultipleChoices;
        }



        public String getStrawPollQuestion()
        {
            return strawPollQuestion;
        }

        public void setStrawPollQuestion(String newStrawPollQuestion)
        {
            strawPollQuestion = newStrawPollQuestion;
        }



        public int getNbrVotesStrawPoll()
        {
            return nbrVotesStrawPoll;
        }

        public void setNbrVotesStrawPoll(int newNbrVotes)
        {
            nbrVotesStrawPoll = newNbrVotes;
        }



        public String getURLStrawPoll()
        {
            return URLStrawPoll;
        }

        public void setURLStrawPoll(String newURLStrawPoll)
        {
            URLStrawPoll = newURLStrawPoll;
        }



        public String getURLSuppression()
        {
            return URLSuppression;
        }


        public void setURLSuppression(String newURLSuppression)
        {
            URLSuppression = newURLSuppression;
        }



        public String getURLResults()
        {
            return URLResults;
        }

        public void setURLResults(String newURLResults)
        {
            URLResults = newURLResults;
        }
    }
}