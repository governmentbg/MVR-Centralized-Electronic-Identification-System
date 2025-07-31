<?xml version="1.0" encoding="UTF-8"?>
<structure version="7" xsltversion="1" cssmode="strict" relativeto="*SPS" encodinghtml="UTF-8" encodingrtf="ISO-8859-1" encodingpdf="UTF-8" embed-images="1">
	<parameters/>
	<schemasources>
		<namespaces>
			<nspair prefix="n1" uri="http://egov.bg/RegiX/MVR/RCH/ForeignIdentityInfoRequest"/>
		</namespaces>
		<schemasources>
			<xsdschemasource name="$XML" main="1" schemafile="..\ForeignIdentityInfoRequestV2.xsd" workingxmlfile="ForeignIdentityInfoRequestV2.xml">
				<xmltablesupport/>
				<textstateicons/>
			</xsdschemasource>
		</schemasources>
	</schemasources>
	<modules/>
	<flags>
		<scripts/>
		<globalparts/>
		<designfragments/>
		<pagelayouts/>
	</flags>
	<scripts>
		<script language="javascript"/>
	</scripts>
	<globalstyles/>
	<mainparts>
		<children>
			<globaltemplate match="/" matchtype="named" parttype="main">
				<children>
					<template match="$XML" matchtype="schemasource">
						<editorproperties elementstodisplay="5"/>
						<children>
							<template match="n1:ForeignIdentityInfoRequest" matchtype="schemagraphitem">
								<editorproperties elementstodisplay="5"/>
								<children>
									<paragraph paragraphtag="h3">
										<properties align="center"/>
										<children>
											<text fixtext="Входни данни за справка за физическо лице – чужденец"/>
										</children>
									</paragraph>
									<table>
										<properties border="0" cellpadding="0" cellspacing="0" width="100%"/>
										<children>
											<tablebody>
												<children>
													<tablerow>
														<children>
															<tablecell>
																<properties width="90"/>
																<children>
																	<template match="n1:IdentifierType" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath=". = &quot;EGN&quot;">
																						<children>
																							<text fixtext="ЕГН">
																								<styles font-style="italic"/>
																							</text>
																						</children>
																					</conditionbranch>
																					<conditionbranch xpath=". = &quot;LNCh&quot;">
																						<children>
																							<text fixtext="ЛНЧ">
																								<styles font-style="italic"/>
																							</text>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																			<text fixtext=":"/>
																		</children>
																	</template>
																</children>
															</tablecell>
															<tablecell>
																<children>
																	<template match="n1:Identifier" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<content>
																				<styles font-weight="bold"/>
																			</content>
																		</children>
																	</template>
																</children>
															</tablecell>
														</children>
													</tablerow>
												</children>
											</tablebody>
										</children>
									</table>
								</children>
							</template>
						</children>
					</template>
				</children>
			</globaltemplate>
		</children>
	</mainparts>
	<globalparts/>
	<pagelayout/>
	<designfragments/>
</structure>
