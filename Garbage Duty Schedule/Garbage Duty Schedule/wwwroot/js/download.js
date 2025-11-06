window.downloadCsv = (filename, base64Content) => {
    if (!filename || !base64Content) {
        return;
    }

    const anchor = document.createElement('a');
    anchor.href = 'data:text/csv;base64,' + base64Content;
    anchor.download = filename;
    anchor.rel = 'noopener';
    anchor.style.display = 'none';
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
};
