const fs = require('fs');
const path = require('path');

function refactorFile(filePath, layout, role) {
    let content = fs.readFileSync(filePath, 'utf8');

    // Skip if already refactored
    if (content.includes('~/Views/Shared')) {
        console.log('Skipping ' + path.basename(filePath));
        return;
    }

    // Extract title
    let titleMatch = content.match(/<title>(.*?)( - .*?\| MediBooking)?<\/title>/);
    let title = titleMatch ? titleMatch[1] : '';

    // Extract active menu
    let activeMatch = content.match(new RegExp('<a href="/' + role + '/([a-zA-Z0-9_]+)" class="sidebar-link active">'));
    let activeMenu = activeMatch ? activeMatch[1] : '';

    // Extract topbar title and subtitle
    let tbTitleMatch = content.match(/<div class="topbar-title"><h4>(.*?)<\/h4>(<p>(.*?)<\/p>)?<\/div>/);
    let tbTitle = tbTitleMatch ? tbTitleMatch[1] : title;
    let tbSubtitle = (tbTitleMatch && tbTitleMatch[3]) ? tbTitleMatch[3] : '';

    // Extract topbar actions
    let tbActionsMatch = content.match(/<div class="topbar-actions">([\s\S]*?)<\/div>\s*<\/header>/);
    let tbActions = tbActionsMatch ? tbActionsMatch[1].trim() : '';

    let parts = content.split(/<div class="content-area page-enter">/);
    if(parts.length < 2) return;
    
    let afterHeader = parts.slice(1).join('<div class="content-area page-enter">');
    
    let scriptsMatch = afterHeader.match(/<script src="\/js\/main\.js\?v=1\.1"><\/script>\s*(<script>[\s\S]*?)<\/body>/);
    let scriptsCode = '';
    let bodyContent = afterHeader;
    
    if (scriptsMatch) {
        scriptsCode = scriptsMatch[1].replace(/<\/script>\s*$/, '</script>');
        bodyContent = afterHeader.substring(0, scriptsMatch.index);
    }
    
    // Clean up closing divs from bodyContent
    bodyContent = bodyContent.replace(/<\/div>\s*<\/div>\s*$/, '');
    bodyContent = bodyContent.replace(/<script src="https:\/\/cdn.jsdelivr[\s\S]*$/, '');

    let newContent = `@{
    Layout = "~/Views/Shared/${layout}.cshtml";
    ViewBag.Title = "${title}";
    ViewBag.ActiveMenu = "${activeMenu}";
    ViewBag.PageTitle = "${tbTitle}";
    ViewBag.PageSubtitle = "${tbSubtitle}";
}
`;

    if (tbActions) {
        newContent += `\n@section TopbarActions {\n    ${tbActions}\n}\n`;
    }

    newContent += `\n${bodyContent.trim()}\n`;

    if (scriptsCode) {
        newContent += `\n@section Scripts {\n    ${scriptsCode.trim()}\n}\n`;
    }

    fs.writeFileSync(filePath, newContent);
    console.log('Refactored ' + path.basename(filePath));
}

const adminFiles = ['Accounts.cshtml', 'Dashboard.cshtml', 'Doctors.cshtml', 'MedicalHistory.cshtml', 'Medicines.cshtml', 'Payments.cshtml', 'Profile.cshtml'];
adminFiles.forEach(f => refactorFile(path.join('d:/Project_Website/WebDatLichKham/Views/Admin', f), '_AdminLayout', 'Admin'));

const doctorFiles = ['CreateRecord.cshtml', 'Examinations.cshtml', 'MedicalHistory.cshtml', 'Profile.cshtml', 'Schedule.cshtml'];
doctorFiles.forEach(f => refactorFile(path.join('d:/Project_Website/WebDatLichKham/Views/Doctor', f), '_DoctorLayout', 'Doctor'));
