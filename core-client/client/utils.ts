import * as alt from 'alt-client'
import * as game from 'natives'

export function degToRad(degrees) {
    return degrees * Math.PI / 180;
}

export function radToDeg(radians) {
    return radians * 180 / Math.PI;
}

function normalizeVector(vector) {
    let mag = game.vmag(vector.x, vector.y, vector.z);

    return {x: vector.x / mag, y: vector.y / mag, z: vector.z / mag};
};

export function rotAnglesToVector(rotation) {
    const z = degToRad(rotation.z);
    const x = degToRad(rotation.x);
    const num = Math.abs(Math.cos(x));

    return { x: -(Math.sin(z) * num), y: (Math.cos(z) * num), z: Math.sin(x) };
}

export function vectorToRotAngles(vector) {
    vector = normalizeVector(vector);
    const ax = radToDeg(Math.asinh(vector.z));
    const az = radToDeg(Math.atan2(vector.x, vector.y));

    return {x: ax, y: 0, z: -az};
}
export const loadAnimDict = (dict: string) => {
    game.requestAnimDict(dict);

    return new Promise((resolve) => {
        const tick = alt.everyTick(() => {
            if (game.hasAnimDictLoaded(dict)) {
                resolve(true);
                alt.clearEveryTick(tick);
            }
        });
    });
};

export async function playAnimation(dict : string, name : string){
    await loadAnimDict(dict)
    game.taskPlayAnim(alt.Player.local, dict, name, 1, 1, -1, 33, 1, false, false, false)
}