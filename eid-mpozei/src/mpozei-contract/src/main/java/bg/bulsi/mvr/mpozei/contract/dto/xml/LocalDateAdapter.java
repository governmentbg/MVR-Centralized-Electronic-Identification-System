package bg.bulsi.mvr.mpozei.contract.dto.xml;

import jakarta.xml.bind.annotation.adapters.XmlAdapter;

import java.time.LocalDate;

public class LocalDateAdapter extends XmlAdapter<String, LocalDate> {

	@Override
	public LocalDate unmarshal(String date) {
		return LocalDate.parse(date);
	}

	@Override
	public String marshal(LocalDate date) {
		return date.toString();
	}
}
