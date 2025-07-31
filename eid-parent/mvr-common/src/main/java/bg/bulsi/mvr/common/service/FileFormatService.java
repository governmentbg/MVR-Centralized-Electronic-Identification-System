package bg.bulsi.mvr.common.service;

import bg.bulsi.mvr.common.dto.ExportModel;

public interface FileFormatService {
    <T> byte[] createFileFromObject(ExportModel model, Class<T> type);
    
    <T extends ExportModel> T createObjectFromXmlString(String text, Class<T> type);
    
    String createXmlStringFromObject(ExportModel model);

    byte[] createJsonFromObject(ExportModel object);

    byte[] createCsvFromObject(ExportModel object);

    byte[] createXmlFromObject(ExportModel object);
}
