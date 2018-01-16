using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuldStrawPoll.Models
{
    public class Answer
    {
        private String answer;
        private int nbrVotesByAnswer;

        public Answer(String newAnswer, int newNbrVotes)
        {
            answer = newAnswer;
            nbrVotesByAnswer = newNbrVotes;
        }

        //GET & SET//
        public String getAnswer()
        {
            return answer;
        }

        public void setAnswer(String newAnswer)
        {
            answer = newAnswer;
        }



        public int getNbrVotesByAnswer()
        {
            return nbrVotesByAnswer;
        }

        //TODO - See if a simple incrementation would do or not
        public void setNbrVotesByAnswer(int newNbrVotesByAnswer)
        {
            nbrVotesByAnswer = newNbrVotesByAnswer;
        }
    }
}