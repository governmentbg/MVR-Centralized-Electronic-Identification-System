# eID Parent README

Update or create a file called settings.xml located in your .m2 folder.
The file should contain the following entries.
Replace ldap_username and ldap_password with your personal username and password.


```
<pluginGroups>
  <pluginGroup>org.sonarsource.scanner.maven</pluginGroup>
</pluginGroups> 

<servers>
  <server>
      <id>deployment</id>
      <username>hudson</username>
       <password>bulsivirtual</password> 
  </server>
  <server>
      <id>BulSIRepository</id>
     <username>admin</username>
      <password>admin123</password>
  </server>
  <server>
      <id>BulSIRepositoryNew</id>
      <username>ldap_username</username>
      <password>ldap_password</password>
  </server> 
  <server>
      <id>BULSI-maven-releases</id>
      <username>ldap_username</username>
      <password>ldap_password</password>
  </server>
  <server>
      <id>BULSI-maven-snapshots</id>
      <username>ldap_username</username>
      <password>ldap_password</password>
  /server>
  
  <server>
      <id>TomcatServer</id>
      <username>admin</username>
      <password>admin</password>
  </server>
  
  <server>
      <id>sonar</id>
      <username>admin</username>
      <password>admin</password>
  </server>
  <server>
      <id>sonar8</id>
      <username>admin</username>
      <password>admin</password>
  </server>
</servers>

<mirrors>
  <mirror>
      <id>maven-default-http-blocker</id>
      <mirrorOf>dummy</mirrorOf>
      <name>Dummy mirror to override default blocking mirror that blocks http</name>
      <url>http://0.0.0.0/</url>
  </mirror>
</mirrors>
```