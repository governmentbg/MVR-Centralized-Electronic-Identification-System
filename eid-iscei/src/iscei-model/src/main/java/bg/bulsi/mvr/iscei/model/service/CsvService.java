package bg.bulsi.mvr.iscei.model.service;

import java.io.IOException;
import java.io.StringWriter;
import java.util.List;

import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;

import com.opencsv.CSVWriter;
import com.opencsv.ICSVWriter;

@Service
public class CsvService {

	public ResponseEntity<Object> generateCsv(List<String[]> data) throws IOException {
        StringWriter stringWriter = new StringWriter();
        try (CSVWriter writer = new CSVWriter(stringWriter, 
        		';',
        		ICSVWriter.NO_QUOTE_CHARACTER,
        		ICSVWriter.DEFAULT_ESCAPE_CHARACTER,
                ICSVWriter.DEFAULT_LINE_END)) {
            writer.writeAll(data);
        }
		
	    HttpHeaders httpHeaders = new HttpHeaders();
	    httpHeaders.set(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=data.csv");
	    httpHeaders.set(HttpHeaders.CONTENT_TYPE, "text/csv; charset=UTF-8");

	    return new ResponseEntity<>(stringWriter.toString(), httpHeaders, HttpStatus.OK);
	}
}
