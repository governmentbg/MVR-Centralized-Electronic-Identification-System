package bg.bulsi.mvr.mpozei.backend.service;

import bg.bulsi.mvr.mpozei.backend.dto.SsevSendMessageDTO;

public interface SsevNotificationService {
    SsevSendMessageDTO sendMessage(SsevSendMessageDTO dto);
}
