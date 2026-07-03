const fs = require('fs');
const path = require('path');

function refactorHomeFile(filePath) {
    let content = fs.readFileSync(filePath, 'utf8');
    if (content.includes('~/Views/Shared/_PublicLayout.cshtml')) {
        console.log('Skipping ' + path.basename(filePath));
        return;
    }

    // Extract title
    let titleMatch = content.match(/<title>(.*?)<\/title>/);
    let title = titleMatch ? titleMatch[1].replace(' - MediBooking', '').trim() : '';

    // Extract active nav link
    let activeMatch = content.match(/<a class="nav-link active" href="\/Home\/([^"]+)">/);
    let activeNav = activeMatch ? activeMatch[1] : (path.basename(filePath) === 'Index.cshtml' ? 'Index' : '');
    if (path.basename(filePath) === 'Index.cshtml' && !activeMatch) activeNav = 'Index';

    let parts = content.split(/<main[^>]*>/);
    if(parts.length < 2) return;
    
    let headAndNav = parts[0];
    let afterMain = parts.slice(1).join('<main>');
    
    // Extract page specific styles if any
    let styles = '';
    let styleMatch = headAndNav.match(/<style>([\s\S]*?)<\/style>/);
    if (styleMatch) {
        styles = styleMatch[0];
    }

    let endParts = afterMain.split(/<\/main>/);
    let bodyContent = endParts[0];
    let afterBody = endParts.slice(1).join('</main>');

    // Extract scripts at the bottom
    let scriptsCode = '';
    let scriptsMatch = afterBody.match(/<script>([\s\S]*?)<\/body>/);
    if (scriptsMatch) {
        scriptsCode = '<script>' + scriptsMatch[1];
        scriptsCode = scriptsCode.replace(/<\/html>\s*$/, '').trim();
    }

    let newContent = `@{
    Layout = "~/Views/Shared/_PublicLayout.cshtml";
    ViewBag.Title = "${title}";
    ViewBag.ActiveNav = "${activeNav}";
}
`;

    if (styles) {
        newContent += `\n@section PageStyles {\n${styles}\n}\n`;
    }

    newContent += `\n${bodyContent.trim()}\n`;

    if (scriptsCode) {
        newContent += `\n@section Scripts {\n${scriptsCode}\n}\n`;
    }

    fs.writeFileSync(filePath, newContent);
    console.log('Refactored ' + path.basename(filePath));
}

const homeFiles = ['About.cshtml', 'BookingSuccess.cshtml', 'ExamResult.cshtml', 'Index.cshtml', 'Lookup.cshtml'];
homeFiles.forEach(f => refactorHomeFile(path.join('d:/Project_Website/WebDatLichKham/Views/Home', f)));
