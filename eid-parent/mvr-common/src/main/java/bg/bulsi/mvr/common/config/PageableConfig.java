package bg.bulsi.mvr.common.config;

import bg.bulsi.mvr.common.util.PageJacksonModule;
import bg.bulsi.mvr.common.util.SortJacksonModule;
import bg.bulsi.mvr.common.util.SortJsonComponent;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class PageableConfig {
    @Bean
    public PageJacksonModule pageJacksonModule() {
        return new PageJacksonModule();
    }

    @Bean
    public SortJacksonModule sortModule() {
        return new SortJacksonModule();
    }

    @Bean
    public SortJsonComponent sortJsonComponent() {
        return new SortJsonComponent();
    }
}
