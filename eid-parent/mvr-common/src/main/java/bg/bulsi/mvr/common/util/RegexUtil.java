package bg.bulsi.mvr.common.util;

public class RegexUtil {
    public static final String PASSWORD_REGEX = "^(?=\\S*[A-Za-z]\\S*[A-Za-z])(?=\\S*[A-Z])(?=\\S*\\d)(?=\\S*[!@#$%^&*()\\-_=+|\\[\\]{};:'\\\",.<>/?])\\S{8,255}$";
    public static final String PHONE_NUMBER_REGEX = "^(?:\\+\\d{3}\\d{9}|0\\d{9})?$|^$";
    public static final String EMAIL_REGEX = "^[a-zA-Z0-9._+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
//    public static final String FIRST_NAME_REGEX = "^([А-Я][а-я]*)(?:[- ]([А-Я][а-я]*))*$";
//    public static final String SECOND_NAME_REGEX = "^([А-Я][а-я]*)(?:[- ]([А-Я][а-я]*))*$|^$";
//    public static final String LAST_NAME_REGEX = "^([А-Я][а-я]*)(?:[- ]([А-Я][а-я]*))*$";
    
    public static final String FIRST_OR_SECOND_NAME_REGEX= "^(?=.{1,40}$)[а-яА-Я\\s-']*$";
    public static final String LAST_NAME_REGEX= "^(?=.{1,60}$)[а-яА-Я\\s-']*$";
    public static final String NAME_REGEX= "^(?=.{1,100}$)[а-яА-Я\\s-']*$";

    public static final String FIRST_OR_SECOND_NAME_LATIN_REGEX= "^(?=.{1,40}$)[a-zA-Z\\s-']*$";
    public static final String LAST_NAME_LATIN_REGEX= "^(?=.{1,60}$)[a-zA-Z\\s-']*$";
    public static final String NAME_LATIN_REGEX= "^(?=.{1,100}$)[a-zA-Z\\s-']*$";

    public static final String CITIZEN_IDENTIFIER_REGEX = "^[0-9]+$";
}
