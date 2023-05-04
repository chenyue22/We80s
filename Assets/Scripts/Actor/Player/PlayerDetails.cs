using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace We80s.GameActor
{
    public enum EducationBackground
    {
        Kindergarden1,                //幼儿园小班
        Kindergarden2,                //幼儿园中班
        Kindergarden3,                //幼儿园大班
        PrimarySchool1,               //小学一年纪                            
        PrimarySchool2,               //小学二年纪             
        PrimarySchool3,               //小学三年纪      
        PrimarySchool4,               //小学四年纪             
        PrimarySchool5,               //小学五年纪             
        PrimarySchool6,               //小学六年纪             
        JuniorHighSchool1,            //初中一年纪
        JuniorHighSchool2,            //初中二年纪
        JuniorHighSchool3,            //初中三年纪
        SeniorHighSchool1,            //高中三年纪
        SeniorHighSchool2,            //高中三年纪
        SeniorHighSchool3,            //高中三年纪
        College1,                     //大学一年级
        College2,                     //大学二年级
        College3,                     //大学三年级
        College4,                     //大学四年级
        Master1,                      //研一
        Master2,                      //研二
        Master3,                      //研三
        Doctor,                       //博士
        TechnicalSecondarySchool1,    //中专一年级
        TechnicalSecondarySchool2,    //中专二年级
        TechnicalSecondarySchool3,    //中专三年级
        JuniorCollege1,               //大专一年级
        JuniorCollege2,               //大专二年级
        JuniorCollege3,               //大专三年级
    }
    
    public struct PlayerDetails
    {
        public int age;
        public int money;
        public EducationBackground educationBackground;
    }
}