<?xml version="1.0" encoding="UTF-8"?>
<structure version="7" xsltversion="1" cssmode="strict" relativeto="*SPS" encodinghtml="UTF-8" encodingrtf="ISO-8859-1" encodingpdf="UTF-8" embed-images="1">
	<parameters/>
	<schemasources>
		<namespaces>
			<nspair prefix="n1" uri="http://egov.bg/RegiX/MVR/RCH//ForeignIdentityInfoResponse"/>
		</namespaces>
		<schemasources>
			<xsdschemasource name="$XML" main="1" schemafile="..\ForeignIdentityInfoResponseV2.xsd" workingxmlfile="ForeignIdentityInfoResponseV2.xml">
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
							<template match="n1:ForeignIdentityInfoResponse" matchtype="schemagraphitem">
								<editorproperties elementstodisplay="5"/>
								<children>
									<paragraph paragraphtag="h3">
										<styles text-align="center"/>
										<children>
											<text fixtext="Министерство на вътрешните работи"/>
											<newline/>
											<text fixtext="Единен регистър на чужденците"/>
										</children>
									</paragraph>
									<paragraph paragraphtag="h2">
										<styles text-align="center"/>
										<children>
											<text fixtext="Справка за физическо лице – чужденец"/>
										</children>
									</paragraph>
									<condition>
										<children>
											<conditionbranch xpath="n1:ReturnInformations/n1:ReturnCode =&apos;0000&apos;">
												<children>
													<condition>
														<children>
															<conditionbranch xpath="string-length( n1:EGN ) +
string-length( n1:LNCh  ) + 
string-length( n1:PersonNames )+
string-length(  n1:BirthDate  )+
string-length( n1:BirthPlace  )+
string-length(  n1:GenderName  )+ string-length( n1:GenderNameLatin  )+string-length( n1:NationalityList  )+string-length(  n1:Statuses )+string-length(  n1:PermanentAddress  )+
string-length(  n1:TemporaryAddress  )+string-length(  n1:PermanentAddressAbroad  )+
string-length(  n1:IdentityDocument  )+
string-length(  n1:TravelDocument  )+
string-length(  n1:Height  )+string-length(  n1:EyesColor  )+string-length(  n1:EyesColorLatin )&gt;0">
																<children>
																	<paragraph>
																		<children>
																			<text fixtext="Лични данни">
																				<styles font-style="italic" font-weight="bold"/>
																			</text>
																		</children>
																	</paragraph>
																	<template match="n1:EGN" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="ЕГН">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:LNCh" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="ЛНЧ">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:PersonNames" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length( . )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Имена">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<template match="n1:FirstName" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:Surname" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=" "/>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:FamilyName" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=" "/>
																																	<content/>
																																</children>
																															</template>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Имена(лат.)">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<template match="n1:FirstNameLatin" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:SurnameLatin" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=" "/>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:LastNameLatin" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=" "/>
																																	<content/>
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
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:GenderName" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Пол">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:GenderNameLatin" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Пол(лат.)">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:Height" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Височина">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:EyesColor" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Цвят на очите">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:EyesColorLatin" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Цвят на очите(лат.)">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:BirthDate" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Дата на раждане">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<autocalc xpath="substring( . , 1 , 2 )"/>
																															<text fixtext="."/>
																															<autocalc xpath="substring( . , 4 , 2 )"/>
																															<text fixtext="."/>
																															<autocalc xpath="substring( . , 7 , 4 )"/>
																															<text fixtext=" г."/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:DeathDate" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(  . ) &gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<children>
																													<tablecell>
																														<properties width="251"/>
																														<children>
																															<text fixtext="Дата на смърт">
																																<styles font-style="italic"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content>
																																<format string="DD.MM.YYYY"/>
																															</content>
																															<text fixtext=" г."/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<newline/>
																	<condition>
																		<children>
																			<conditionbranch xpath="string-length( n1:BirthPlace )&gt;0">
																				<children>
																					<paragraph>
																						<children>
																							<text fixtext="Месторождение">
																								<styles font-style="italic" font-weight="bold"/>
																							</text>
																						</children>
																					</paragraph>
																					<template match="n1:BirthPlace" matchtype="schemagraphitem">
																						<editorproperties elementstodisplay="5"/>
																						<children>
																							<template match="n1:CountryName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Държава">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:CountryNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Държава(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:DistrictName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Област">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:MunicipalityName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<table>
																										<properties border="0" width="100%"/>
																										<children>
																											<tablebody>
																												<children>
																													<tablerow>
																														<properties valign="top"/>
																														<children>
																															<tablecell headercell="1">
																																<properties align="left" width="250px"/>
																																<children>
																																	<text fixtext="Община">
																																		<styles font-style="italic" font-weight="normal"/>
																																	</text>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<content/>
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
																							<template match="n1:TerritorialUnitName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Населено място">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																						</children>
																					</template>
																				</children>
																			</conditionbranch>
																		</children>
																	</condition>
																	<template match="n1:IdentityDocument" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<paragraph>
																								<children>
																									<text fixtext="Данни за документ за самоличност">
																										<styles font-style="italic" font-weight="bold"/>
																									</text>
																								</children>
																							</paragraph>
																							<template match="n1:DocumentType" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Тип документ">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:DocumentTypeLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Тип документ(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IdentityDocumentNumber" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Номер на документ за самоличност">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssueDate" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Дата на издаване">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content>
																																						<format string="DD.MM.YYYY"/>
																																					</content>
																																					<text fixtext=" г."/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssuePlace" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Място на издаване">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssuePlaceLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Място на издаване(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssuerName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Издаден от">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssuerNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Издаден от(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:ValidDate" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Валиден до">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content>
																																						<format string="DD.MM.YYYY"/>
																																					</content>
																																					<text fixtext=" г."/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:StatusDate" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(  .  ) &gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="251"/>
																																				<children>
																																					<text fixtext="Дата на актуален статус">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content>
																																						<format string="DD.MM.YYYY"/>
																																					</content>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:StatusReasonCyrillic" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<newline/>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length( . ) &gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="256"/>
																																				<children>
																																					<text fixtext="Причина за актуален статус - текст на кирилица">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																									<newline/>
																								</children>
																							</template>
																							<template match="n1:RPTypeOfPermit" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(  .  ) &gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="257"/>
																																				<children>
																																					<text fixtext="Тип пребиваване в док. Разрешение за пребиваване">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:RPRemarks" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(  .  ) &gt;0">
																												<children>
																													<table>
																														<properties border="1" cellpadding="5px"/>
																														<styles border-collapse="collapse"/>
																														<children>
																															<tableheader>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell headercell="1">
																																				<children>
																																					<text fixtext="Забележки в док. Разрешение за преб.">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tableheader>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<children>
																																					<template match="n1:RPRemark" matchtype="schemagraphitem">
																																						<editorproperties elementstodisplay="5"/>
																																						<children>
																																							<content/>
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
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:TravelDocument" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<paragraph>
																								<children>
																									<text fixtext="Документ за задгранично пътуване">
																										<styles font-style="italic" font-weight="bold"/>
																									</text>
																								</children>
																							</paragraph>
																							<template match="n1:DocumentType" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Тип документ">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:DocumentTypeLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Тип документ(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:TravelDocumentSeries" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Серия на документ ">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:TravelDocumentNumber" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Номер на документ">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssueDate" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Дата на издаване">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content>
																																						<format string="DD.MM.YYYY"/>
																																					</content>
																																					<text fixtext=" г."/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssuePlace" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Място на издаване">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssuePlaceLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Място на издаване(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssuerName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Издаден от">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:IssuerNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Издаден от(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:ValidDate" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Валиден до">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content>
																																						<format string="DD.MM.YYYY"/>
																																					</content>
																																					<text fixtext=" г."/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:StatusDate" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(  .  ) &gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="251"/>
																																				<children>
																																					<text fixtext="Дата на актуален статус">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content>
																																						<format string="DD.MM.YYYY"/>
																																					</content>
																																					<text fixtext=" г."/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:StatusReasonCyrillic" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length( . ) &gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="253"/>
																																				<children>
																																					<text fixtext="Причина за актуален статус">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:NationalityList" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<paragraph>
																								<children>
																									<text fixtext="Националност">
																										<styles font-style="italic" font-weight="bold"/>
																									</text>
																								</children>
																							</paragraph>
																							<table>
																								<properties border="1" cellpadding="5px"/>
																								<styles border-collapse="collapse"/>
																								<children>
																									<tableheader>
																										<children>
																											<tablerow>
																												<children>
																													<tablecell headercell="1">
																														<children>
																															<text fixtext="Код">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell headercell="1">
																														<children>
																															<text fixtext="Държава">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell headercell="1">
																														<children>
																															<text fixtext="Държава(лат.)">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tableheader>
																									<tablebody>
																										<children>
																											<template match="n1:Nationality" matchtype="schemagraphitem">
																												<editorproperties elementstodisplay="5"/>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<children>
																																	<template match="n1:NationalityCode" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
																																		</children>
																																	</template>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:NationalityName" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
																																		</children>
																																	</template>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:NationalityNameLatin" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
																																		</children>
																																	</template>
																																</children>
																															</tablecell>
																														</children>
																													</tablerow>
																												</children>
																											</template>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<condition>
																		<children>
																			<conditionbranch xpath="string-length(n1:PermanentAddress )&gt;0">
																				<children>
																					<template match="n1:PermanentAddress" matchtype="schemagraphitem">
																						<editorproperties elementstodisplay="5"/>
																						<children>
																							<paragraph>
																								<children>
																									<text fixtext="Постоянен адрес">
																										<styles font-style="italic" font-weight="bold"/>
																									</text>
																								</children>
																							</paragraph>
																							<template match="n1:DistrictName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Област">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:DistrictNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" cellpadding="3" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Област(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:MunicipalityName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Община">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:MunicipalityNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Община(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:SettlementName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Населено място">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:SettlementCode" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Код на населено място">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:SettlementNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Населено място(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Адрес">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<template match="n1:LocationName" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:LocationNameLatin" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext="("/>
																																	<content/>
																																	<text fixtext=")"/>
																																</children>
																															</template>
																															<template match="n1:BuildingNumber" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=", номер "/>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:Entrance" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=", вх. "/>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:Floor" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=", ет. "/>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:Apartment" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=", ап. "/>
																																	<content/>
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
																			</conditionbranch>
																		</children>
																	</condition>
																	<condition>
																		<children>
																			<conditionbranch xpath="string-length( n1:TemporaryAddress  )&gt;0">
																				<children>
																					<template match="n1:TemporaryAddress" matchtype="schemagraphitem">
																						<editorproperties elementstodisplay="5"/>
																						<children>
																							<paragraph>
																								<children>
																									<text fixtext="Настоящ адрес">
																										<styles font-style="italic" font-weight="bold"/>
																									</text>
																								</children>
																							</paragraph>
																							<template match="n1:DistrictName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Област">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:DistrictNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" cellpadding="3" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Област(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:MunicipalityName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Община">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:MunicipalityNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Община(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:SettlementName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Населено място">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:SettlementCode" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Код на населено място">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<template match="n1:SettlementNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="250px"/>
																																				<children>
																																					<text fixtext="Населено място(лат.)">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<properties valign="top"/>
																												<children>
																													<tablecell headercell="1">
																														<properties align="left" width="250px"/>
																														<children>
																															<text fixtext="Адрес">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<template match="n1:LocationName" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:LocationNameLatin" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext="("/>
																																	<content/>
																																	<text fixtext=")"/>
																																</children>
																															</template>
																															<template match="n1:BuildingNumber" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=" "/>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:Entrance" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=" вх. "/>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:Floor" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=" ет. "/>
																																	<content/>
																																</children>
																															</template>
																															<template match="n1:Apartment" matchtype="schemagraphitem">
																																<editorproperties elementstodisplay="5"/>
																																<children>
																																	<text fixtext=" ап. "/>
																																	<content/>
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
																			</conditionbranch>
																		</children>
																	</condition>
																	<condition>
																		<children>
																			<conditionbranch xpath="string-length( n1:PermanentAddressAbroad  )&gt;0">
																				<children>
																					<template match="n1:PermanentAddressAbroad" matchtype="schemagraphitem">
																						<editorproperties elementstodisplay="5"/>
																						<children>
																							<newline/>
																							<paragraph>
																								<children>
																									<text fixtext="Постоянен адрес в чужбина">
																										<styles font-style="italic" font-weight="bold"/>
																									</text>
																								</children>
																							</paragraph>
																							<template match="n1:NationalityCode" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="count( . )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="264"/>
																																				<children>
																																					<text fixtext="Код на държава">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																									<newline/>
																								</children>
																							</template>
																							<newline/>
																							<template match="n1:NationalityName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="261"/>
																																				<children>
																																					<text fixtext="Наименование на държава">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<newline/>
																							<template match="n1:NationalityNameLatin" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="268"/>
																																				<children>
																																					<text fixtext="Наименование на държава на латиница">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<newline/>
																							<template match="n1:SettlementName" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<properties valign="top"/>
																																		<children>
																																			<tablecell headercell="1">
																																				<properties align="left" width="260"/>
																																				<children>
																																					<text fixtext="Населено място">
																																						<styles font-style="italic" font-weight="normal"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<newline/>
																							<template match="n1:Street" matchtype="schemagraphitem">
																								<editorproperties elementstodisplay="5"/>
																								<children>
																									<condition>
																										<children>
																											<conditionbranch xpath="string-length(. )&gt;0">
																												<children>
																													<table>
																														<properties border="0" width="100%"/>
																														<children>
																															<tablebody>
																																<children>
																																	<tablerow>
																																		<children>
																																			<tablecell>
																																				<properties width="258"/>
																																				<children>
																																					<text fixtext="Данни за адрес">
																																						<styles font-style="italic"/>
																																					</text>
																																				</children>
																																			</tablecell>
																																			<tablecell>
																																				<children>
																																					<content/>
																																				</children>
																																			</tablecell>
																																		</children>
																																	</tablerow>
																																</children>
																															</tablebody>
																														</children>
																													</table>
																												</children>
																											</conditionbranch>
																										</children>
																									</condition>
																								</children>
																							</template>
																							<newline/>
																						</children>
																					</template>
																				</children>
																			</conditionbranch>
																		</children>
																	</condition>
																	<template match="n1:Employer" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(  .  ) &gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<children>
																													<tablecell>
																														<properties width="262"/>
																														<children>
																															<text fixtext="Работодател">
																																<styles font-style="italic"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:Position" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length( . )&gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<children>
																													<tablecell>
																														<properties width="260"/>
																														<children>
																															<text fixtext="Длъжност">
																																<styles font-style="italic"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<template match="n1:PositionLatin" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(  .  ) &gt;0">
																						<children>
																							<table>
																								<properties border="0" width="100%"/>
																								<children>
																									<tablebody>
																										<children>
																											<tablerow>
																												<children>
																													<tablecell>
																														<properties width="261"/>
																														<children>
																															<text fixtext="Длъжност(лат.)">
																																<styles font-style="italic"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell>
																														<children>
																															<content/>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																		</children>
																	</template>
																	<newline/>
																	<newline/>
																	<newline/>
																	<template match="n1:Statuses" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<condition>
																				<children>
																					<conditionbranch xpath="string-length(. )&gt;0">
																						<children>
																							<paragraph>
																								<children>
																									<text fixtext="Статус на пребиваване">
																										<styles font-style="italic" font-weight="bold"/>
																									</text>
																								</children>
																							</paragraph>
																							<table>
																								<properties border="1" cellpadding="5px"/>
																								<styles border-collapse="collapse"/>
																								<children>
																									<tableheader>
																										<children>
																											<tablerow>
																												<children>
																													<tablecell headercell="1">
																														<children>
																															<text fixtext="Статус">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell headercell="1">
																														<children>
																															<text fixtext="Статус(лат.)">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell headercell="1">
																														<children>
																															<text fixtext="Валиден от">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																													<tablecell headercell="1">
																														<children>
																															<text fixtext="Валиден до">
																																<styles font-style="italic" font-weight="normal"/>
																															</text>
																														</children>
																													</tablecell>
																												</children>
																											</tablerow>
																										</children>
																									</tableheader>
																									<tablebody>
																										<children>
																											<template match="n1:Status" matchtype="schemagraphitem">
																												<editorproperties elementstodisplay="5"/>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<children>
																																	<template match="n1:StatusName" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
																																		</children>
																																	</template>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:StatusNameLatin" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
																																		</children>
																																	</template>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:DateFrom" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content>
																																				<format string="DD.MM.YYYY"/>
																																			</content>
																																			<text fixtext=" г."/>
																																		</children>
																																	</template>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:DateTo" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content>
																																				<format string="DD.MM.YYYY"/>
																																			</content>
																																			<text fixtext=" г."/>
																																		</children>
																																	</template>
																																</children>
																															</tablecell>
																														</children>
																													</tablerow>
																												</children>
																											</template>
																										</children>
																									</tablebody>
																								</children>
																							</table>
																						</children>
																					</conditionbranch>
																				</children>
																			</condition>
																			<newline/>
																			<template match="n1:StatusLawReason" matchtype="schemagraphitem">
																				<editorproperties elementstodisplay="5"/>
																				<children>
																					<paragraph>
																						<children>
																							<text fixtext="Статус на правното основание">
																								<styles font-weight="bold"/>
																							</text>
																						</children>
																					</paragraph>
																					<condition>
																						<children>
																							<conditionbranch xpath="string-length(   n1:Status  )&gt;0">
																								<children>
																									<table>
																										<properties border="0" width="100%"/>
																										<children>
																											<tablebody>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<properties width="263"/>
																																<children>
																																	<text fixtext="Статус на правното основание">
																																		<styles font-style="italic"/>
																																	</text>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:Status" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
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
																							</conditionbranch>
																						</children>
																					</condition>
																					<condition>
																						<children>
																							<conditionbranch xpath="string-length(  n1:StatusLatin  ) &gt;0">
																								<children>
																									<table>
																										<properties border="0" width="100%"/>
																										<children>
																											<tablebody>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<properties width="263"/>
																																<children>
																																	<text fixtext="Статус на правното основание(лат)">
																																		<styles font-style="italic"/>
																																	</text>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:StatusLatin" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
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
																							</conditionbranch>
																						</children>
																					</condition>
																					<condition>
																						<children>
																							<conditionbranch xpath="string-length(  n1:Code  ) &gt;0">
																								<children>
																									<table>
																										<properties border="0" width="100%"/>
																										<children>
																											<tablebody>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<properties width="266"/>
																																<children>
																																	<text fixtext="Код">
																																		<styles font-style="italic"/>
																																	</text>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:Code" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
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
																							</conditionbranch>
																						</children>
																					</condition>
																					<newline/>
																				</children>
																			</template>
																			<template match="n1:StatusDocument" matchtype="schemagraphitem">
																				<editorproperties elementstodisplay="5"/>
																				<children>
																					<newline/>
																					<paragraph>
																						<children>
																							<text fixtext="Документ за предоставянее/ отказ/ отнемане на статут">
																								<styles font-weight="bold"/>
																							</text>
																						</children>
																					</paragraph>
																					<condition>
																						<children>
																							<conditionbranch xpath="string-length(  n1:DocumentType ) &gt;0">
																								<children>
																									<table>
																										<properties border="0" width="100%"/>
																										<children>
																											<tablebody>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<properties width="265"/>
																																<children>
																																	<text fixtext="Вид на документ">
																																		<styles font-style="italic"/>
																																	</text>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:DocumentType" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
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
																							</conditionbranch>
																						</children>
																					</condition>
																					<condition>
																						<children>
																							<conditionbranch xpath="string-length(  n1:DocumentTypeLatin  ) &gt;0">
																								<children>
																									<table>
																										<properties border="0" width="100%"/>
																										<children>
																											<tablebody>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<properties width="265"/>
																																<children>
																																	<text fixtext="Вид на документ(лат)">
																																		<styles font-style="italic"/>
																																	</text>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:DocumentTypeLatin" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
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
																							</conditionbranch>
																						</children>
																					</condition>
																					<condition>
																						<children>
																							<conditionbranch xpath="string-length( n1:DocumentNumber ) &gt; 0">
																								<children>
																									<table>
																										<properties border="0" width="100%"/>
																										<children>
																											<tablebody>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<properties width="265"/>
																																<children>
																																	<text fixtext="Статус на правното основание">
																																		<styles font-style="italic"/>
																																	</text>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:DocumentNumber" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content/>
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
																							</conditionbranch>
																						</children>
																					</condition>
																					<condition>
																						<children>
																							<conditionbranch xpath="string-length(  n1:StatusDocumentDate  ) &gt;0">
																								<children>
																									<table>
																										<properties border="0" width="100%"/>
																										<children>
																											<tablebody>
																												<children>
																													<tablerow>
																														<children>
																															<tablecell>
																																<properties width="268"/>
																																<children>
																																	<text fixtext="Дата на документ за статут">
																																		<styles font-style="italic"/>
																																	</text>
																																</children>
																															</tablecell>
																															<tablecell>
																																<children>
																																	<template match="n1:StatusDocumentDate" matchtype="schemagraphitem">
																																		<editorproperties elementstodisplay="5"/>
																																		<children>
																																			<content>
																																				<format string="DD.MM.YYYY"/>
																																			</content>
																																			<text fixtext=" г."/>
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
																							</conditionbranch>
																						</children>
																					</condition>
																				</children>
																			</template>
																			<newline/>
																			<template match="n1:Category" matchtype="schemagraphitem">
																				<editorproperties elementstodisplay="5"/>
																				<children>
																					<newline/>
																					<paragraph>
																						<children>
																							<text fixtext="Категории">
																								<styles font-weight="bold"/>
																							</text>
																						</children>
																					</paragraph>
																					<table>
																						<properties border="1" cellpadding="5px"/>
																						<styles border-collapse="collapse"/>
																						<children>
																							<tableheader>
																								<children>
																									<tablerow>
																										<children>
																											<tablecell headercell="1">
																												<children>
																													<text fixtext="Код">
																														<styles font-style="italic"/>
																													</text>
																												</children>
																											</tablecell>
																											<tablecell headercell="1">
																												<children>
																													<text fixtext="Категория(лат.)"/>
																												</children>
																											</tablecell>
																											<tablecell headercell="1">
																												<children>
																													<text fixtext="Категория"/>
																												</children>
																											</tablecell>
																										</children>
																									</tablerow>
																								</children>
																							</tableheader>
																							<tablebody>
																								<children>
																									<tablerow>
																										<children>
																											<tablecell>
																												<children>
																													<template match="n1:Code" matchtype="schemagraphitem">
																														<editorproperties elementstodisplay="5"/>
																														<children>
																															<content/>
																														</children>
																													</template>
																												</children>
																											</tablecell>
																											<tablecell>
																												<children>
																													<template match="n1:CategoryLatin" matchtype="schemagraphitem">
																														<editorproperties elementstodisplay="5"/>
																														<children>
																															<content/>
																														</children>
																													</template>
																												</children>
																											</tablecell>
																											<tablecell>
																												<children>
																													<template match="n1:CategoryCyrillic" matchtype="schemagraphitem">
																														<editorproperties elementstodisplay="5"/>
																														<children>
																															<content/>
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
																	<newline/>
																	<template match="n1:Picture" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<paragraph>
																				<children>
																					<text fixtext="Снимка">
																						<styles font-style="italic" font-weight="bold"/>
																					</text>
																				</children>
																			</paragraph>
																			<image>
																				<target>
																					<xpath value="concat(&quot;data:image/png;base64,&quot;, string(.))"/>
																				</target>
																				<imagesource>
																					<xpath value="concat(&quot;data:image/png;base64,&quot;, string(.))"/>
																				</imagesource>
																			</image>
																		</children>
																	</template>
																	<template match="n1:IdentitySignature" matchtype="schemagraphitem">
																		<editorproperties elementstodisplay="5"/>
																		<children>
																			<paragraph>
																				<children>
																					<text fixtext="Личен подпис">
																						<styles font-style="italic" font-weight="bold"/>
																					</text>
																				</children>
																			</paragraph>
																			<image>
																				<target>
																					<xpath value="concat(&quot;data:image/png;base64,&quot;, string(.))"/>
																				</target>
																				<imagesource>
																					<xpath value="concat(&quot;data:image/png;base64,&quot;, string(.))"/>
																				</imagesource>
																			</image>
																		</children>
																	</template>
																</children>
															</conditionbranch>
															<conditionbranch>
																<children>
																	<text fixtext="Не са намерени данни в реиистъра за подадените входни параметри."/>
																</children>
															</conditionbranch>
														</children>
													</condition>
												</children>
											</conditionbranch>
											<conditionbranch>
												<children>
													<template match="n1:ReturnInformations" matchtype="schemagraphitem">
														<editorproperties elementstodisplay="5"/>
														<children>
															<paragraph>
																<children>
																	<text fixtext="Възникна грешка при търсене на данните в регистъра">
																		<styles font-style="italic" font-weight="bold"/>
																	</text>
																</children>
															</paragraph>
															<template match="n1:ReturnCode" matchtype="schemagraphitem">
																<editorproperties elementstodisplay="5"/>
																<children>
																	<table>
																		<properties border="0" width="100%"/>
																		<children>
																			<tablebody>
																				<children>
																					<tablerow>
																						<properties valign="top"/>
																						<children>
																							<tablecell headercell="1">
																								<properties align="left" width="250px"/>
																								<children>
																									<text fixtext="Код на грешка">
																										<styles font-style="italic" font-weight="normal"/>
																									</text>
																								</children>
																							</tablecell>
																							<tablecell>
																								<children>
																									<content/>
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
															<template match="n1:Info" matchtype="schemagraphitem">
																<editorproperties elementstodisplay="5"/>
																<children>
																	<table>
																		<properties border="0" width="100%"/>
																		<children>
																			<tablebody>
																				<children>
																					<tablerow>
																						<properties valign="top"/>
																						<children>
																							<tablecell headercell="1">
																								<properties align="left" width="250px"/>
																								<children>
																									<text fixtext="Съобщение за грешка">
																										<styles font-style="italic" font-weight="normal"/>
																									</text>
																								</children>
																							</tablecell>
																							<tablecell>
																								<children>
																									<content/>
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
											</conditionbranch>
										</children>
									</condition>
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
