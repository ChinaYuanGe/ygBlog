function febase(data) {
    return btoa(encodeURIComponent(data));
}
function fdbase(encoded) {
    return decodeURIComponent(atob(encoded));
}