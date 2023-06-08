export const getMUISeverity = (severity: string) => {
    switch (severity) {
        case 'Info':
            return 'info';
        case 'Warning':
            return 'warning';
        case 'Error':
            return 'error';
    }
}