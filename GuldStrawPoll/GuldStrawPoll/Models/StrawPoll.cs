using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuldStrawPoll.Models
{
    public class StrawPoll
    {
        //ATTRIBUTES
        private bool multipleChoices;
        private String strawPollQuestion;
        private int nbrVotesStrawPoll;
        private String URLStrawPoll;
        private String URLDeletion;
        private String URLResults;
        private bool isActive;
        private Guid GuidStrawPoll = Guid.NewGuid();

        //CONSTRUCTORS
        public StrawPoll(){}

        public StrawPoll(String newStrawPollQuestion, bool newMultipleChoices, int newNbrVotes, bool newIsActive)
        {
            multipleChoices = newMultipleChoices;
            strawPollQuestion = newStrawPollQuestion;
            nbrVotesStrawPoll = newNbrVotes;

            isActive = newIsActive;
        }

        public StrawPoll(String newStrawPollQuestion, bool newMultipleChoices, int newNbrVotes, String newURLStrawPoll, String newURLDeletion, String newURLResults, bool newIsActive)
        {
            multipleChoices = newMultipleChoices;
            strawPollQuestion = newStrawPollQuestion;
            nbrVotesStrawPoll = newNbrVotes;
            URLStrawPoll = newURLStrawPoll;
            URLDeletion = newURLDeletion;
            URLResults = newURLResults;

            isActive = newIsActive;
        }

        //METHODS//
        public String generateURLStrawPoll(int ID)
        {
            return "/Home/VotePage?ID=" + ID.ToString();
        }

        public String generateURLDeletion()
        {
            return "/Home/DeletionPage?GuidStrawPoll=" + GuidStrawPoll;
        }

        public String generateURLResults(int ID)
        {
            return "/Home/ResultPage?ID=" + ID.ToString();
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



        public bool getIsActive()
        {
            return isActive;
        }

        public void setIsActive(bool stateStrawPoll)
        {
            isActive = stateStrawPoll;
        }



        public Guid getGuidStrawPoll()
        {
            return GuidStrawPoll;
        }

        public void setGuidStrawPoll(Guid newGuid)
        {
            GuidStrawPoll = newGuid;
        }
    }
}