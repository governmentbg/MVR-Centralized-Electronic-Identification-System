package bg.bulsi.mvr.mpozei.backend.dto.misep;

import lombok.Data;

import java.util.List;

@Data
public class MisepClientResponse {
    private Department department;
    private List<Unite> unites;

    @Data
    public static class Department {
        private String type;
        private Uid uid;
        private String name;
    }

    @Data
    public static class Uid {
        private String type;
        private String value;
    }

    @Data
    public static class Unite {
        private String type;
        private Uid uid;
        private String name;
        private Info info;
        private boolean isActive;
    }

    @Data
    public static class Info {
        private MisepBankAccount bankAccount;
    }
}
