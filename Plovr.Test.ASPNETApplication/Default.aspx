<%@ Page Language="C#" %>
<!doctype html>
<html>
	<head>
		<title>Plovr.NET - Test ASPNET Application</title>

		<link rel="stylesheet" href="/css/demo.css" />
		<link rel="stylesheet" href="/css/closure/datepicker.css" />
		<link rel="stylesheet" href="/css/closure/inputdatepicker.css" /> 
		
		<!-- we still include jquery as we normally would -->
		<script src="/js/jquery-1.6.2.min.js"></script>
	</head>
	<body> 
		<!-- use the path from the web.config -->
		<script src="<%= Plovr.Utilities.GetIncludePath() %>"></script>

		<!-- include the jquery test via passing an id into plovr, you can also pass mode -->
		<script src="/plovr.net/compile?id=test-jquery&mode=advanced"></script>

		<!-- how you pass an id -->
		<script src="/plovr.net/compile?id=test-tooltip&mode=advanced"></script>

<%--		<script src="/plovr.axd?id=plovr-MessageSystem"></script> --%>
	</body>
</html>