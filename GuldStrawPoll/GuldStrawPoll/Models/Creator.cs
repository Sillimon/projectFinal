using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuldStrawPoll.Models
{
    public class Creator
    {
        private String loginCreator;

        public Creator(String login)
        {
            loginCreator = login;
        }

        //GET & SET//

        public String getLoginCreator()
        {
            return loginCreator;            
        }

        public void setLoginCreator(String login)
        {
            loginCreator = login;
        }


    }
}