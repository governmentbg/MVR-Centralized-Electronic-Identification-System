package bg.bulsi.mvr.pdf_generator;

import java.nio.charset.StandardCharsets;

import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.thymeleaf.TemplateEngine;
import org.thymeleaf.spring6.SpringTemplateEngine;
import org.thymeleaf.templatemode.TemplateMode;
import org.thymeleaf.templateresolver.ClassLoaderTemplateResolver;
import org.thymeleaf.templateresolver.ITemplateResolver;

@Configuration
@ConditionalOnProperty(
	    value="mvr.pdf-generator.enabled", 
	    havingValue = "true")
public class PdfGeneratorConfig {

	@Bean
	public PdfGenerator pdfGenerator() {
		return new PdfGenerator();
	}
	
	@Bean
	public TemplateEngine pdfTemplateEngine() {
		final SpringTemplateEngine templateEngine = new SpringTemplateEngine();
		templateEngine.addTemplateResolver(htmlPdfTemplateResolver());
		return templateEngine;
	}

	private ITemplateResolver htmlPdfTemplateResolver() {
		final ClassLoaderTemplateResolver templateResolver = new ClassLoaderTemplateResolver();
		templateResolver.setOrder(Integer.valueOf(1));
		// templateResolver.setResolvablePatterns(Collections.singleton("html/*"));
		templateResolver.setPrefix("/templates/pdf/");
		templateResolver.setSuffix(".html");
		templateResolver.setTemplateMode(TemplateMode.HTML);
		templateResolver.setCharacterEncoding(StandardCharsets.UTF_8.toString());
		templateResolver.setCacheable(false);

		return templateResolver;
	}
}
