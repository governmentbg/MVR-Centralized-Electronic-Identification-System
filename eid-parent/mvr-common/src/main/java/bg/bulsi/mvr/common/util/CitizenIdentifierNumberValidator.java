package bg.bulsi.mvr.common.util;

import lombok.extern.slf4j.Slf4j;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

@Slf4j
public class CitizenIdentifierNumberValidator {

    public static boolean isValidCheckSum(int checkSum, String number) {
        return checkSum == Character.getNumericValue(number.charAt(9));
    }

    public static boolean isValidLength(String number) {
        return number.length() == 10;
    }

    public static boolean containsIntegerOnly(String number) {
        return number.matches("[0-9]+");
    }

    public static boolean validateLNCH(String number) {
        if (!isValidLength(number) || !containsIntegerOnly(number)) {
            return false;
        }
        int checkSum = computeLNCHCheckSum(number);
        return isValidCheckSum(checkSum, number);
    }

    public static int computeLNCHCheckSum(String number) {
        int checkSum = 0;
        int[] weights = {21, 19, 17, 13, 11, 9, 7, 3, 1};

        for (int i = 0; i < weights.length; ++i) {
            checkSum += weights[i] * Character.getNumericValue(number.charAt(i));
        }

        // If the remainder of checkSum is a number less than 10, it becomes the check digit.
        // If it is equal to 10, the check digit is 0.
        checkSum %= 10;
        return checkSum;
    }

    public static boolean validateEGN(String egn) {
        if (!isValidLength(egn) || !containsIntegerOnly(egn)) {
            return false;
        }
        int checkSum = computeEGNCheckSum(egn);
        return getDateByEGN(egn) != null && isValidCheckSum(checkSum, egn);
    }

    public static int computeEGNCheckSum(String egn) {
        int checkSum = 0;
        int[] weights = {2, 4, 8, 5, 10, 9, 7, 3, 6};

        for (int i = 0; i < weights.length; ++i) {
            checkSum += weights[i] * Character.getNumericValue(egn.charAt(i));
        }

        checkSum %= 11;
        checkSum %= 10;

        return checkSum;
    }

    public static Date getDateByEGN(String egn) {
        if (egn.length() != 10) {
            return null;
        }

        int controlValueYear = Integer.parseInt(egn.substring(0, 2));
        int controlValueMonth = Integer.parseInt(egn.substring(2, 4));
        String controlValueDate = egn.substring(4, 6);

        int month = controlValueMonth;
        int baseYear = 1900;
        if (controlValueMonth > 40) {
            month -= 40;
            baseYear = 2000;
        } else if (controlValueMonth > 20) {
            baseYear = 1800;
            month -= 20;
        }
        int year = baseYear + controlValueYear;

        String dateString = String.format("%d/%02d/%s", year, month, controlValueDate);

        try {
            SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd");
            dateFormat.setLenient(false);
            return dateFormat.parse(dateString);
        } catch (ParseException e) {
            return null;
        }
    }
}