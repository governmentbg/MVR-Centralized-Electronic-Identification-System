package bg.bulsi.mvr.common.service;

import bg.bulsi.mvr.common.dto.ExportModel;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import com.google.gson.GsonBuilder;
import com.opencsv.CSVWriter;
import jakarta.xml.bind.JAXBContext;
import jakarta.xml.bind.JAXBException;
import jakarta.xml.bind.Marshaller;
import jakarta.xml.bind.Unmarshaller;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.io.*;
import java.lang.reflect.Field;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;

@Slf4j
@Service
@RequiredArgsConstructor
public class FileFormatServiceImpl implements FileFormatService {

    /**
     * TODO: check for xxe
     */
    @Override
    public <T> byte[] createFileFromObject(ExportModel model, Class<T> type) {
        try (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
            StringWriter writer = new StringWriter();
            JAXBContext context = JAXBContext.newInstance(type);
            Marshaller marshaller = context.createMarshaller();
            marshaller.setProperty(Marshaller.JAXB_FRAGMENT, true);

            marshaller.marshal(model, writer);
            outputStream.write(writer.toString().getBytes(Charset.defaultCharset()));
            return outputStream.toByteArray();
        } catch (JAXBException | IOException e) {
            log.error(".createFileFromXmlModel() Cannot create XML file");
            throw new FaultMVRException(e);
        }
    }

	@Override
	public <T extends ExportModel> T createObjectFromXmlString(String xmlText, Class<T> type) {
		JAXBContext jaxbContext;
		try {
			StringReader reader = new StringReader(xmlText);
			jaxbContext = JAXBContext.newInstance(type);
			Unmarshaller unmarshaller = jaxbContext.createUnmarshaller();
			
			return type.cast(unmarshaller.unmarshal(reader));
		} catch (JAXBException e) {
            log.error(".createXmlModelFromString() Cannot create Java object from the provided text");
            throw new FaultMVRException(e);
		}
	}

	@Override
	public String createXmlStringFromObject(ExportModel object) {
        return mapExportModelToStream(object).toString(StandardCharsets.UTF_8);
	}

    @Override
    public byte[] createJsonFromObject(ExportModel object) {
        try (ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
            GsonBuilder builder = new GsonBuilder();
            builder.serializeNulls();
            String jsonString = builder.create().toJson(object);

            baos.write(jsonString.getBytes(StandardCharsets.UTF_8));
            return baos.toByteArray();
        } catch (Exception e) {
            throw new FaultMVRException(e);
        }
    }

    @Override
    public byte[] createCsvFromObject(ExportModel object) {
        try (ByteArrayOutputStream baos = new ByteArrayOutputStream();
             OutputStreamWriter outputStreamWriter = new OutputStreamWriter(baos, StandardCharsets.UTF_8);
             CSVWriter csvWriter = new CSVWriter(outputStreamWriter)) {

            Field[] fields = ExportModel.class.getDeclaredFields();

            String[] headers = Arrays.stream(fields)
                    .map(Field::getName)
                    .toArray(String[]::new);
            csvWriter.writeNext(headers);

            String[] data = Arrays.stream(fields)
                    .map(field -> {
                        field.setAccessible(true);
                        try {
                            return field.get(object).toString();
                        } catch (IllegalAccessException e) {
                            log.error(e.getMessage());
                            return "";
                        }
                    })
                    .toArray(String[]::new);
            csvWriter.writeNext(data);

            csvWriter.flush();
            return baos.toByteArray();
        } catch (Exception e) {
            throw new FaultMVRException(e);
        }
    }

    @Override
    public byte[] createXmlFromObject(ExportModel object) {
        return mapExportModelToStream(object).toByteArray();
    }

    private ByteArrayOutputStream mapExportModelToStream(ExportModel object) {
        try (ByteArrayOutputStream outputStream = new ByteArrayOutputStream()) {
            StringWriter writer = new StringWriter();
            JAXBContext context = JAXBContext.newInstance(object.getClass());
            Marshaller marshaller = context.createMarshaller();
            marshaller.setProperty(Marshaller.JAXB_FRAGMENT, true);

            marshaller.marshal(object, writer);
            outputStream.write(writer.toString().getBytes(StandardCharsets.UTF_8));
            return outputStream;
        } catch (JAXBException | IOException e) {
            log.error(".createXmlFromObject() Cannot create XML from object");
            throw new FaultMVRException(e);
        }
    }
}
