package bg.bulsi.mvr.mpozei.backend.util;

import jakarta.xml.bind.annotation.adapters.XmlAdapter;

import java.time.OffsetDateTime;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;

public class XmlOffsetDateTimeAdapter extends XmlAdapter<String, OffsetDateTime> {

    private final DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd");

    @Override
    public String marshal(OffsetDateTime v) throws Exception {
        synchronized (formatter) {
            return formatter.format(v);
        }
    }

    @Override
    public OffsetDateTime unmarshal(String v) throws Exception {
        synchronized (formatter) {
            return ZonedDateTime.parse(v, formatter).toOffsetDateTime();
        }
    }

}
