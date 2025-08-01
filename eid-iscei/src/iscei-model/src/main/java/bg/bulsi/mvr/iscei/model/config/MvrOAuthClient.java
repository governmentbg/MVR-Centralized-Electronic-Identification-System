package bg.bulsi.mvr.iscei.model.config;

import java.util.UUID;

import lombok.Data;

@Data
public class MvrOAuthClient {

    private String name;
   	private UUID systemId;
   	private String systemName;
}
