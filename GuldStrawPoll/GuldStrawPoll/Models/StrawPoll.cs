﻿using System;
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
        private String URLDeletion;
        private String URLResults;

        public StrawPoll(){}

        public StrawPoll(String newStrawPollQuestion, bool newMultipleChoices, int newNbrVotes)
        {
            multipleChoices = newMultipleChoices;
            strawPollQuestion = newStrawPollQuestion;
            nbrVotesStrawPoll = newNbrVotes;
        }

        public StrawPoll(String newStrawPollQuestion, bool newMultipleChoices, int newNbrVotes, String newURLStrawPoll, String newURLDeletion, String newURLResults)
        {
            multipleChoices = newMultipleChoices;
            strawPollQuestion = newStrawPollQuestion;
            nbrVotesStrawPoll = newNbrVotes;
            URLStrawPoll = newURLStrawPoll;
            URLDeletion = newURLDeletion;
            URLResults = newURLResults;
        }

        //METHODS//
        public String generateURLStrawPoll(int ID)
        {
            return "/Home/VotePage?ID=" + ID.ToString();
        }

        public String generateURLDeletion(int ID)
        {
            return "/Home/DeletionPage?ID=" + ID.ToString();
        }

        public String generateURLResults(int ID)
        {
            return "/Home/ResultsPage?ID=" + ID.ToString();
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



        public String getURLDeletion()
        {
            return URLDeletion;
        }


        public void setURLDeletion(String newURLDeletion)
        {
            URLDeletion = newURLDeletion;
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