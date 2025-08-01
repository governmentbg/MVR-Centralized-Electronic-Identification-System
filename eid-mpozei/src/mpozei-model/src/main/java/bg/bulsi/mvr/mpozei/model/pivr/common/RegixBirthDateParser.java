package bg.bulsi.mvr.mpozei.model.pivr.common;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.time.format.DateTimeParseException;
import java.util.Optional;

public class RegixBirthDateParser {
	/**
	 * birthday can be in the following formats
	 * - dd/MM/YYYY
	 * - yyyy-MM-dd
	 * - dd.MM.yyyy or dd/MM/yyyy
	 *   - dd.MM.yyyy - full date
	 *   - 00.mm.yyyy (00.11.1988)
	 *   - 00.00.yyyy  (00.00.1999)
	 */
	public static Optional<LocalDate> parseBirthDateParsed(String birthDate) {
		if (birthDate == null || birthDate.isEmpty()) {
			return Optional.empty();
		}

		try {
			// Try parsing the date directly
			DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd"); // Adjust format as needed
			LocalDate parsedDate = LocalDate.parse(birthDate, formatter);
			return Optional.of(parsedDate);
		} catch (DateTimeParseException e) {
			try {
				DateTimeFormatter formatter2 = DateTimeFormatter.ofPattern("dd/MM/yyyy");
				return Optional.of(LocalDate.parse(birthDate, formatter2));
			} catch (DateTimeParseException ex) {
				// If parsing fails, attempt to parse using the custom logic
				return parseDate(birthDate);
			}
		}
	}

	private static Optional<LocalDate> parseDate(String input) {
		String[] parts = input.split("\\."); // Split by '.'
		if (parts.length != 3) {
			 parts = input.split("/");
             if (parts.length != 3){
     			return Optional.empty();
             }
		}

		try {
			int day = parts[0].equals("00") ? 1 : Integer.parseInt(parts[0]);
			int month = parts[1].equals("00") ? 1 : Integer.parseInt(parts[1]);
			int year = Integer.parseInt(parts[2]);

			return Optional.of(LocalDate.of(year, month, day));
		} catch (NumberFormatException | DateTimeParseException e) {
			return Optional.empty();
		}
	}
}
