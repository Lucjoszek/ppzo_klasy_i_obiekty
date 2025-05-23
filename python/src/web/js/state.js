/** @typedef {import('./types.js').User} User */

/**
 * Global application state object.
 * Holds user data and UI-related state.
 */
export const appState = {
    /** 
     * The user data.
     * @type {User | null} 
     */
    user: null,

    /** 
     * Event listener for rename button click, or null if not set.
     * @type {function | null} 
     */
    renameClickListener: null,

    /** Event listener for playlist title input changes, or null if not set.
     * @type {function | null} 
     */
    renameInputListener: null,

    /** 
     * Stores the interval ID used to update track position during playback.
     * @type {funciton | null} 
     */
    intervailId: null
}