package bg.bulsi.mvr.pdf_generator;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.Map;

import org.springframework.beans.factory.annotation.Autowired;
import org.thymeleaf.TemplateEngine;
import org.thymeleaf.context.Context;
import org.xhtmlrenderer.pdf.ITextRenderer;

import com.lowagie.text.DocumentException;
import com.lowagie.text.pdf.BaseFont;

import lombok.extern.slf4j.Slf4j;

/**
 * Class responsible for generating PDF by providing templateName and
 * templateVariables for the templates.
 */
@Slf4j
public class PdfGenerator {

	@Autowired
	private TemplateEngine pdfTemplateEngine;

	public byte[] generatePdf(String templateName, Map<String, Object> templateVariables) {
		String htmlContent = this.process(templateName, templateVariables);

		return this.generatePdfFromHtml(htmlContent);
	}
	
	private byte[] generatePdfFromHtml(String htmlContent) {
		ByteArrayOutputStream outputStream = new ByteArrayOutputStream();

		ITextRenderer renderer = new ITextRenderer();
		try {
			renderer.getFontResolver().addFont("static/fonts/Blogger Sans.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
		} catch (DocumentException | IOException e) {
			log.error("An error occurred while processing the document " + e);
		}
		
		renderer.setDocumentFromString(htmlContent);
		renderer.layout();
		renderer.createPDF(outputStream);

		return outputStream.toByteArray();
	}

	private String process(String templateName, Map<String, Object> templateVariables) {
		final Context context = new Context();
		context.setVariables(templateVariables);
		
		return this.pdfTemplateEngine.process(templateName, context);
	}
}
