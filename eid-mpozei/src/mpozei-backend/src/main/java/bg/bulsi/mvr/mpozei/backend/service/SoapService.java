package bg.bulsi.mvr.mpozei.backend.service;

public interface SoapService {
    byte[] prepareDataAsByteArray(String chipSerialNumber, String holderId);
}
