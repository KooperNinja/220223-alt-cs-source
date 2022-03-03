import * as alt from 'alt-client'
import { RGBA, Vector3 } from 'alt-client'
import * as game from 'natives'
import { playAnimation } from './utils'

let placing : boolean = false
let type: string 
interface PlacingData {
    active: boolean,
    type?: string,
    animDict?: string,
    animName?: string,
}


alt.on("localMetaChange", (key, newVal : string) => {
    let plData : PlacingData = JSON.parse(newVal)

    if (key == "ObjectPlacing:Status") {
        placing = plData.active
        type = plData.type
        return;
    }
    if (key == "ObjectPlacing:Planting") {
        alt.toggleGameControls(!plData.active);
        alt.log(plData.animDict, plData.animName)
        if (plData.active) {
            //Dictionary: amb@world_human_gardener_plant@male@base | Animation: base
            playAnimation(plData.animDict, plData.animName)
        }
        if (!plData.active){ 
            game.clearPedTasks(p)
        }
    }
})

alt.on("keydown", (key) => {
    if (!placing) return
    if (key === 69) {
        alt.emitServer("ObjectPlacing:Validate", type, pos, vec)
    }
})

alt.onServer("ObjectPlacing:ValidateFail", () => {
    alt.log("Vali Failed recived")
    color = failColor;
    alt.setTimeout(() => {
        color = mainColor
    }, 1000)
})

//Credtis to Phill#1649

const p : alt.Player = alt.Player.local
let pos : Vector3
let vec : Vector3 
const mainColor : RGBA = {r: 28, g: 107, b: 74, a: 255}
const failColor : RGBA = {r: 255, g: 0, b: 0, a: 255}
let color : RGBA = mainColor


alt.setInterval(() => {
    if (!placing) return
    let [_,_hit,_endCoords,_surfaceNormal,_materialHash,_entityHit] = getRaycast();
    if (_hit) {
        
        game.drawSphere(_endCoords.x, _endCoords.y, _endCoords.z, 0.1, color.r, color.g, color.b, color.a);
        pos = new Vector3(_endCoords.x, _endCoords.y, _endCoords.z)
        vec = _surfaceNormal
    }
    
}, 5)

function getRaycast() {
    let start = game.getFinalRenderedCamCoord();
    let rot = game.getFinalRenderedCamRot(2);
    let fvector = GetDirectionFromRotation(rot);
    let frontOf = new alt.Vector3((start.x + (fvector.x * 10)), (start.y + (fvector.y * 10)), (start.z + (fvector.z * 10)));
    let raycast = game.startExpensiveSynchronousShapeTestLosProbe(game.getGameplayCamCoord().x - 0.5, game.getGameplayCamCoord().y - 0.6, game.getGameplayCamCoord().z, frontOf.x, frontOf.y, frontOf.z, -1, p, 4);
    let rayResult = game.getShapeTestResultIncludingMaterial(raycast);
    return rayResult;
}

function GetDirectionFromRotation(rotation) {
    var z = rotation.z * (Math.PI / 180.0);
    var x = rotation.x * (Math.PI / 180.0);
    var num = Math.abs(Math.cos(x));

    return new alt.Vector3(
        (-Math.sin(z) * num),
        (Math.cos(z) * num),
        Math.sin(x)
    );
}