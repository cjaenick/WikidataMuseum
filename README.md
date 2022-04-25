This is a Unity Project developed to strengthen my thesis on showcasing how Wikidata information can be used. In this application, developed for Android devices, images 
of artificial intelligence researchers which have been added as Vuforia Engine Image Targets can be scanned to display information about them, creating a museum for
AI researchers. The information which is displayed is retrived from Wikidata's SPARQL endpoint, the event handler for each researcher contains queries asking Wikidata
for information about the researcher, such as their name, description, employer and notable work. To add more researchers to the application, one needs to add a Vuforia
Image Target, link an image of the researcher to this target, copy and paste one of the event handlers onto the new target, and rewrite the query and parts where the 
specific GameObject is referenced to match the new target.
