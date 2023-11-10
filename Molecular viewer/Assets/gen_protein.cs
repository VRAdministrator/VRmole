using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Linq;



public class gen_protein : MonoBehaviour
{
    //public InputActionProperty stick;
    public grab_script left_grab_info,right_grab_info;
    public GameObject atom_model;
    public GameObject bond_model;
    
    private GameObject temp_atom;
    private List<GameObject> Cs,Ns,Os,Ss,atoms,bonds_obs,bones_obs;
    private bool pre_up_turn,mid_turn,pregrabbed;
    private int frame,mode,protein_index,pre_index;
    //private float cur_scale;


    public ref struct CustomRef
    {
        public Vector3 Pos;
        public string El;
    }

    private static float str_to_float(string str,int start){
        int len=6;
        for (;str[start]==32;){
            len--;
            start++;
        }
        return float.Parse(str.Substring(start,len), System.Globalization.CultureInfo.InvariantCulture)/100;
    }
    // Start is called before the first frame update

    const double DefaultBondingRadius = 2.001;

    void Start()
    {

        var __ElementIndex= new Dictionary<string,int>(){{"H",0},{"h",0},{"D",0},{"d",0},{"T",0},{"t",0},{"He",2},{"HE",2},{"he",2},{"Li",3},{"LI",3},{"li",3},{"Be",4},{"BE",4},{"be",4},{"B",5},{"b",5},{"C",6},{"c",6},{"N",7},{"n",7},{"O",8},{"o",8},{"F",9},{"f",9},{"Ne",10},{"NE",10},{"ne",10},{"Na",11},{"NA",11},{"na",11},{"Mg",12},{"MG",12},{"mg",12},{"Al",13},{"AL",13},{"al",13},{"Si",14},{"SI",14},{"si",14},{"P",15},{"p",15},{"S",16},{"s",16},{"Cl",17},{"CL",17},{"cl",17},{"Ar",18},{"AR",18},{"ar",18},{"K",19},{"k",19},{"Ca",20},{"CA",20},{"ca",20},{"Sc",21},{"SC",21},{"sc",21},{"Ti",22},{"TI",22},{"ti",22},{"V",23},{"v",23},{"Cr",24},{"CR",24},{"cr",24},{"Mn",25},{"MN",25},{"mn",25},{"Fe",26},{"FE",26},{"fe",26},{"Co",27},{"CO",27},{"co",27},{"Ni",28},{"NI",28},{"ni",28},{"Cu",29},{"CU",29},{"cu",29},{"Zn",30},{"ZN",30},{"zn",30},{"Ga",31},{"GA",31},{"ga",31},{"Ge",32},{"GE",32},{"ge",32},{"As",33},{"AS",33},{"as",33},{"Se",34},{"SE",34},{"se",34},{"Br",35},{"BR",35},{"br",35},{"Kr",36},{"KR",36},{"kr",36},{"Rb",37},{"RB",37},{"rb",37},{"Sr",38},{"SR",38},{"sr",38},{"Y",39},{"y",39},{"Zr",40},{"ZR",40},{"zr",40},{"Nb",41},{"NB",41},{"nb",41},{"Mo",42},{"MO",42},{"mo",42},{"Tc",43},{"TC",43},{"tc",43},{"Ru",44},{"RU",44},{"ru",44},{"Rh",45},{"RH",45},{"rh",45},{"Pd",46},{"PD",46},{"pd",46},{"Ag",47},{"AG",47},{"ag",47},{"Cd",48},{"CD",48},{"cd",48},{"In",49},{"IN",49},{"in",49},{"Sn",50},{"SN",50},{"sn",50},{"Sb",51},{"SB",51},{"sb",51},{"Te",52},{"TE",52},{"te",52},{"I",53},{"i",53},{"Xe",54},{"XE",54},{"xe",54},{"Cs",55},{"CS",55},{"cs",55},{"Ba",56},{"BA",56},{"ba",56},{"La",57},{"LA",57},{"la",57},{"Ce",58},{"CE",58},{"ce",58},{"Pr",59},{"PR",59},{"pr",59},{"Nd",60},{"ND",60},{"nd",60},{"Pm",61},{"PM",61},{"pm",61},{"Sm",62},{"SM",62},{"sm",62},{"Eu",63},{"EU",63},{"eu",63},{"Gd",64},{"GD",64},{"gd",64},{"Tb",65},{"TB",65},{"tb",65},{"Dy",66},{"DY",66},{"dy",66},{"Ho",67},{"HO",67},{"ho",67},{"Er",68},{"ER",68},{"er",68},{"Tm",69},{"TM",69},{"tm",69},{"Yb",70},{"YB",70},{"yb",70},{"Lu",71},{"LU",71},{"lu",71},{"Hf",72},{"HF",72},{"hf",72},{"Ta",73},{"TA",73},{"ta",73},{"W",74},{"w",74},{"Re",75},{"RE",75},{"re",75},{"Os",76},{"OS",76},{"os",76},{"Ir",77},{"IR",77},{"ir",77},{"Pt",78},{"PT",78},{"pt",78},{"Au",79},{"AU",79},{"au",79},{"Hg",80},{"HG",80},{"hg",80},{"Tl",81},{"TL",81},{"tl",81},{"Pb",82},{"PB",82},{"pb",82},{"Bi",83},{"BI",83},{"bi",83},{"Po",84},{"PO",84},{"po",84},{"At",85},{"AT",85},{"at",85},{"Rn",86},{"RN",86},{"rn",86},{"Fr",87},{"FR",87},{"fr",87},{"Ra",88},{"RA",88},{"ra",88},{"Ac",89},{"AC",89},{"ac",89},{"Th",90},{"TH",90},{"th",90},{"Pa",91},{"PA",91},{"pa",91},{"U",92},{"u",92},{"Np",93},{"NP",93},{"np",93},{"Pu",94},{"PU",94},{"pu",94},{"Am",95},{"AM",95},{"am",95},{"Cm",96},{"CM",96},{"cm",96},{"Bk",97},{"BK",97},{"bk",97},{"Cf",98},{"CF",98},{"cf",98},{"Es",99},{"ES",99},{"es",99},{"Fm",100},{"FM",100},{"fm",100},{"Md",101},{"MD",101},{"md",101},{"No",102},{"NO",102},{"no",102},{"Lr",103},{"LR",103},{"lr",103},{"Rf",104},{"RF",104},{"rf",104},{"Db",105},{"DB",105},{"db",105},{"Sg",106},{"SG",106},{"sg",106},{"Bh",107},{"BH",107},{"bh",107},{"Hs",108},{"HS",108},{"hs",108},{"Mt",109},{"MT",109},{"mt",109}};
        var __ElementBondThresholds=new Dictionary<int,double>(){{0,1.42},{ 1,1.42},{ 3,2.7},{ 4,2.7},{ 6,1.75},{ 7,1.6},{ 8,1.52},{ 11,2.7},{ 12,2.7},{ 13,2.7},{ 14,1.9},{ 15,1.9},{ 16,1.9},{ 17,1.8},{ 19,2.7},{ 20,2.7},{ 21,2.7},{ 22,2.7},{ 23,2.7},{ 24,2.7},{ 25,2.7},{ 26,2.7},{ 27,2.7},{ 28,2.7},{ 29,2.7},{ 30,2.7},{ 31,2.7},{ 33,2.68},{ 37,2.7},{ 38,2.7},{ 39,2.7},{ 40,2.7},{ 41,2.7},{ 42,2.7},{ 43,2.7},{ 44,2.7},{ 45,2.7},{ 46,2.7},{ 47,2.7},{ 48,2.7},{ 49,2.7},{ 50,2.7},{ 55,2.7},{ 56,2.7},{ 57,2.7},{ 58,2.7},{ 59,2.7},{ 60,2.7},{ 61,2.7},{ 62,2.7},{ 63,2.7},{ 64,2.7},{ 65,2.7},{ 66,2.7},{ 67,2.7},{ 68,2.7},{ 69,2.7},{ 70,2.7},{ 71,2.7},{ 72,2.7},{ 73,2.7},{ 74,2.7},{ 75,2.7},{ 76,2.7},{ 77,2.7},{ 78,2.7},{ 79,2.7},{ 80,2.7},{ 81,2.7},{ 82,2.7},{ 83,2.7},{ 87,2.7},{ 88,2.7},{ 89,2.7},{ 90,2.7},{ 91,2.7},{ 92,2.7},{ 93,2.7},{ 94,2.7},{ 95,2.7},{ 96,2.7},{ 97,2.7},{ 98,2.7},{ 99,2.7},{ 100,2.7},{ 101,2.7},{ 102,2.7},{ 103,2.7},{ 104,2.7},{ 105,2.7},{ 106,2.7},{ 107,2.7},{ 108,2.7},{ 109,2.88}};
        var __ElementPairThresholds=new Dictionary<int,double>{{0,0.8},{ 20,1.31},{ 27,1.3},{ 35,1.3},{ 44,1.05},{ 54,1},{ 60,1.84},{ 72,1.88},{ 84,1.75},{ 85,1.56},{ 86,1.76},{ 98,1.6},{ 99,1.68},{ 100,1.63},{ 112,1.55},{ 113,1.59},{ 114,1.36},{ 129,1.45},{ 144,1.6},{ 170,1.4},{ 180,1.55},{ 202,2.4},{ 222,2.24},{ 224,1.91},{ 225,1.98},{ 243,2.02},{ 269,2},{ 293,1.9},{ 480,2.3},{ 512,2.3},{ 544,2.3},{ 612,2.1},{ 629,1.54},{ 665,1},{ 813,2.6},{ 854,2.27},{ 894,1.93},{ 896,2.1},{ 937,2.05},{ 938,2.06},{ 981,1.62},{ 1258,2.68},{ 1309,2.33},{ 1484,1},{ 1763,2.14},{ 1823,2.48},{ 1882,2.1},{ 1944,1.72},{ 2380,2.34},{ 3367,2.44},{ 3733,2.11},{ 3819,2.6},{ 3821,2.36},{ 4736,2.75},{ 5724,2.73},{ 5959,2.63},{ 6519,2.84},{ 6750,2.87},{ 8991,2.81}};
        List<string> MetalsSet=new List<string>{"LI", "NA", "K", "RB", "CS", "FR", "BE", "MG", "CA", "SR", "BA", "RA","AL", "GA", "IN", "SN", "TL", "PB", "BI", "SC", "TI", "V", "CR", "MN", "FE", "CO", "NI", "CU", "ZN", "Y", "ZR", "NB", "MO", "TC", "RU", "RH", "PD", "AG", "CD", "LA", "HF", "TA", "W", "RE", "OS", "IR", "PT", "AU", "HG", "AC", "RF", "DB", "SG", "BH", "HS", "MT", "CE", "PR", "ND", "PM", "SM", "EU", "GD", "TB", "DY", "HO", "ER", "TM", "YB", "LU", "TH", "PA", "U", "NP", "PU", "AM", "CM", "BK", "CF", "ES", "FM", "MD", "NO", "LR"};

        (List<int>,List<double>) quary_3d(Vector3 pos,List<Vector3> positions,double max,int start){
            var atoms=new List<int>();
            var distances=new List<double>();
            for (int i=start+1;i<positions.Count;i++){
                Vector3 temp_pos=positions[i];
                double dx=Math.Abs(temp_pos.x-pos.x);
                if (dx>max){
                    continue;
                }
                double dy=Math.Abs(temp_pos.y-pos.y);
                if (dy>max){
                    continue;
                }
                double dz=Math.Abs(temp_pos.z-pos.z);
                if (dz>max){
                    continue;
                }
                double dis=Math.Sqrt(dx*dx+dy*dy+dz*dz);
                if (max<dis || dis==0){
                    continue;
                }
                atoms.Add(i);
                distances.Add(dis);
            }
            return (atoms,distances);
        }

        int idx(string str){
            int i=__ElementIndex[str];
            if (i==0){
                return -1;
            }
            return i;
        }

        bool isHydrogen(int i){
            if (i==0){
                return true;
            }
            return false;
        }

        double theshold(int i){
            if (i<0){
                return DefaultBondingRadius;
            }
            double r = __ElementBondThresholds[i];
            if (r==0){
                return DefaultBondingRadius;
            }
            return r;
        }

        int pair(int a,int b){
            if (a<b){
                return (a+b)*(a+b+1)/2+b;
            }
            return (a+b)*(a+b+1)/2+a;
        }

        double pairThreshold(int i,int j){
            if (i < 0 || j < 0) return -1;
            double r;
            __ElementPairThresholds.TryGetValue(pair(i,j),out r);
            if (r==0) return -1;
            return r;
        }

        (List<int>,List<int>) compute_bonds(List<string> els,List<Vector3> positions){
            int total_atoms=els.Count;
            List<int> atomA=new List<int>();
            List<int> atomB=new List<int>();
            for (int ai=0;ai<total_atoms;ai++){
                int aei=idx(els[ai]);
                var atom_infos=quary_3d(positions[ai],positions,0.02,ai);
                bool isHa=isHydrogen(aei);
                double thesholdA=theshold(aei);
                bool metalA=MetalsSet.Contains(els[ai]);
                for (int ni=0;ni<atom_infos.Item1.Count;ni++){
                    int bi=atom_infos.Item1[ni];
                    int bei=idx(els[bi]);
                    bool isHb=isHydrogen(bei);
                    if (isHa && isHb){
                        continue;
                    }
                    if (isHa || isHb){
                        if (atom_infos.Item2[ni]<1.15){//maybe 1.15
                            atomA.Add(ai);
                            atomB.Add(bi);
                            //bond_types.Add(1);
                        }
                        continue;
                    }
                    double thresholdAB=pairThreshold(aei,bei);
                    double pairingThreshold;
                    if (thresholdAB>0){
                        pairingThreshold=thresholdAB;
                    }else{
                        if (bei<0){
                            pairingThreshold=thesholdA;
                        }else{
                            double thesholdB=theshold(bei);
                            if (thesholdA>thesholdB){
                                pairingThreshold=thesholdA;
                            }else{
                                pairingThreshold=thesholdB;
                            }

                        }
                    }
                    bool metalB=MetalsSet.Contains(els[bi]);
                    if (atom_infos.Item2[ni]<=pairingThreshold){
                        atomA.Add(ai);
                        atomB.Add(bi);
                    }
                }
            }
            return (atomA,atomB);
        }

        (List<Vector3>,List<int>) get_bonds(List<string> els,List<Vector3> positions){
            var atom_lists=compute_bonds(els,positions);
            var atomsA=atom_lists.Item1;
            var atomsB=atom_lists.Item2;
            List<Vector3> bond_positions=new List<Vector3>(new Vector3[atomsA.Count]);
            for (int i=0;i<atomsA.Count;i++){
                bond_positions[i]=(positions[atomsA[i]]+positions[atomsB[i]])/2;
            }
            return (bond_positions,atomsA);
        }


        Transform trans=GetComponent<Transform>();
        Vector3 proir_position=trans.position;
        trans.position=new Vector3(0,0,0);
        atoms=new List<GameObject>();
        Cs=new List<GameObject>();
        Ns=new List<GameObject>();
        Os=new List<GameObject>();
        Ss=new List<GameObject>();
        TextAsset textFile=(TextAsset)Resources.Load("1mbo");
        string text=textFile.text;
        int start=0;
        int len_string=text.Length-81;
        float total=0;
        Vector3 center_pt=new Vector3();
        //Vector3 offset=new Vector3(-.7f,1.0f,.8f);
        List<string> eles=new List<string>();
        List<Vector3> atom_pos=new List<Vector3>();
        List<Vector3> amino_pos=new List<Vector3>();
        int preres=0;
        float div=0.0F;
        Vector3 cur_amino_pos=new Vector3();
        for (;start<len_string;){  
            if (text.Substring(start,4)=="ATOM"){
                temp_atom=Instantiate(atom_model,trans);
                Vector3 temp_pos=new Vector3(str_to_float(text,start+32),str_to_float(text,start+40),str_to_float(text,start+48));
                atom_pos.Add(temp_pos);
                center_pt+=temp_pos;
                temp_atom.transform.position=temp_pos;
                temp_atom.transform.localScale=new Vector3(0.01f,0.01f,0.01f);
                eles.Add(text.Substring(start+77,1));
                atoms.Add(temp_atom);
                switch (text[start+77]){
                    case (char)67:
                    Cs.Add(temp_atom);
                    temp_atom.GetComponent<Renderer>().material.color=Color.gray;
                    break;
                    case (char)78:
                    Ns.Add(temp_atom);
                    temp_atom.GetComponent<Renderer>().material.color=Color.blue;
                    break;
                    case (char)79:
                    Os.Add(temp_atom);
                    temp_atom.GetComponent<Renderer>().material.color=Color.red;
                    break;
                    case (char)83:
                    Ss.Add(temp_atom);
                    temp_atom.GetComponent<Renderer>().material.color=Color.green;
                    break;
                }
                int res_start=22;
                for (;text[start+res_start]==" "[0];res_start++){}
                int res_index=int.Parse(text.Substring(start+res_start,26-res_start));
                if (res_index==preres){
                    cur_amino_pos+=temp_pos;
                    div++;
                }else{
                    amino_pos.Add(cur_amino_pos/div);
                    cur_amino_pos=temp_pos;
                    div=1.0F;
                    preres=res_index;
                }
                total++;
                start+=81;
            }else{
                for (;text[start]!=10;){
                    start++;
                }
                start++;
            }
        }
        amino_pos.Add(cur_amino_pos/div);
        center_pt/=total;
        for (int i=0;i<atoms.Count;i++){
            atoms[i].transform.position-=center_pt;
            atom_pos[i]-=center_pt;
        }
        for (int i=0;i<amino_pos.Count;i++){
            amino_pos[i]-=center_pt;
        }

        var bond_info=get_bonds(eles,atom_pos);
        var bond_positions=bond_info.Item1;
        var base_atoms=bond_info.Item2;
        bonds_obs=new List<GameObject>(new GameObject[bond_positions.Count]);
        for (int i=0;i<bond_positions.Count;i++){
            GameObject bond=Instantiate(bond_model,trans);
            bond.transform.position=bond_positions[i];
            bond.transform.LookAt(atoms[base_atoms[i]].transform);
            bond.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            bonds_obs[i]=bond;
        }
        int size=amino_pos.Count-1;
        Vector3 dif=new Vector3();
        bones_obs=new List<GameObject>(new GameObject[size]);
        for (int i=0;i<size;i++){
            GameObject bone=Instantiate(bond_model,trans);
            dif=amino_pos[i]-amino_pos[i+1];
            bone.transform.localScale=new Vector3(0.003F,MathF.Sqrt(dif.x*dif.x+dif.y*dif.y+dif.z*dif.z)*0.5F,0.003F);
            bone.transform.position=(amino_pos[i]+amino_pos[i+1])/2;
            bone.transform.LookAt(amino_pos[i]);
            bone.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
            bones_obs[i]=bone;
            bone.GetComponent<Renderer>().enabled=false;
        }
        int spacer=Cs.Count()/100;
        for (int i=0;i<Cs.Count;i++){
            if (i%spacer!=0){
                continue;
            }
            SphereCollider temp_collider=gameObject.AddComponent<SphereCollider>();
            temp_collider.radius=0.05F;
            temp_collider.center=Cs[i].transform.position;
        }
        trans.position=proir_position;
        /*mesh version get to work later
        Mesh cartoon_mesh=new Mesh();
        cartoon_mesh.name="cartoon_mesh";
        GetComponent<MeshFilter>().mesh=cartoon_mesh;
        cartoon_mesh.vertices=new Vector3[amino_pos.Count<<2];
        //create four verts for each point in the mesh
        int I=0;
        for (int i=0;i<amino_pos.Count;i++){
            cartoon_mesh.vertices[I]=amino_pos[i];
            cartoon_mesh.vertices[I+1]=amino_pos[i];
            cartoon_mesh.vertices[I+2]=amino_pos[i];
            cartoon_mesh.vertices[I+3]=amino_pos[i];
            cartoon_mesh.vertices[I].x+=0.001F;
            cartoon_mesh.vertices[I+1].y+=0.001F;
            cartoon_mesh.vertices[I+2].x-=0.001F;
            cartoon_mesh.vertices[I+3].y-=0.001F;
            I+=4;
        }
        int size=I*6-24;
        I=0;
        cartoon_mesh.triangles=new int[size];
        for (int i=0;i<size;i+=24){
            cartoon_mesh.triangles[i]=I;
            cartoon_mesh.triangles[i+1]=I+1;
            cartoon_mesh.triangles[i+2]=I+4;
            cartoon_mesh.triangles[i+3]=I+4;
            cartoon_mesh.triangles[i+4]=I+5;
            cartoon_mesh.triangles[i+5]=I+1;
            cartoon_mesh.triangles[i+6]=I+1;
            cartoon_mesh.triangles[i+7]=I+2;
            cartoon_mesh.triangles[i+8]=I+5;
            cartoon_mesh.triangles[i+9]=I+5;
            cartoon_mesh.triangles[i+10]=I+6;
            cartoon_mesh.triangles[i+11]=I+2;
            cartoon_mesh.triangles[i+12]=I+2;
            cartoon_mesh.triangles[i+13]=I+3;
            cartoon_mesh.triangles[i+14]=I+6;
            cartoon_mesh.triangles[i+15]=I+6;
            cartoon_mesh.triangles[i+16]=I+7;
            cartoon_mesh.triangles[i+17]=I+3;
            cartoon_mesh.triangles[i+18]=I+3;
            cartoon_mesh.triangles[i+19]=I;
            cartoon_mesh.triangles[i+20]=I+7;
            cartoon_mesh.triangles[i+21]=I+7;
            cartoon_mesh.triangles[i+22]=I+4;
            cartoon_mesh.triangles[i+23]=I;
            I+=4;
        }
        GetComponent<Renderer>().enabled=false;
        */


    }
    void LateUpdate(){
        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        Vector2 stick_vec=new Vector2();
        if (leftHandDevices.Count==1){
            var left_con=leftHandDevices[0];
            bool padvalue;
            bool A_pressed=Input.GetKeyDown(KeyCode.JoystickButton0);
            if ((left_con.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondary2DAxisClick, out padvalue)&&padvalue)||A_pressed){
                Transform trans=GetComponent<Transform>();
                trans.position=new Vector3(0.0F,1.5F,1.0F);
                trans.eulerAngles=new Vector3(112.0F,-33.0F,-91.0F);
                var rb=GetComponent<Rigidbody>();
                rb.angularVelocity=new Vector3(0.0F,0.0F,0.0F);
                rb.velocity=new Vector3(0.0F,0.0F,0.0F);
                left_grab_info.before_grabbed=false;
                right_grab_info.before_grabbed=false;
            }
            left_con.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out stick_vec);
        }
        bool change=false;
        //Vector2 stick_vec=stick.action.ReadValue<Vector2>();
        if (stick_vec.y>0.5&&(frame>90||!pre_up_turn||mid_turn)){
            pre_up_turn=true;
            mid_turn=false;
            pre_index=protein_index;
            protein_index++;
            change=true;
            frame=0;
        }
        else if (stick_vec.y<-0.5&&(frame>90||pre_up_turn||mid_turn)){
            pre_up_turn=false;
            mid_turn=false;
            pre_index=protein_index;
            protein_index--;
            change=true;
            frame=0;
        }
        else if (!mid_turn&&Mathf.Round(stick_vec.y*10)==0){
            mid_turn=true;
        }

        protein_index=(protein_index+3)%3;
        if (change){
            switch (protein_index){
                case 0:
                for (int i=0;i<bonds_obs.Count;i++){
                    bonds_obs[i].GetComponent<Renderer>().enabled=true;
                }
                for (int i=0;i<atoms.Count;i++){
                    atoms[i].transform.localScale=new Vector3(0.01f,0.01f,0.01f);
                    atoms[i].GetComponent<Renderer>().enabled=true;
                }
                if (pre_index==2){
                    for (int i=0;i<bones_obs.Count;i++){
                        bones_obs[i].GetComponent<Renderer>().enabled=false;
                    }
                }
                break;
                case 1:
                if (pre_index==0){
                    for (int i=0;i<bonds_obs.Count;i++){
                        bonds_obs[i].GetComponent<Renderer>().enabled=false;
                    }
                }else{
                    for (int i=0;i<atoms.Count;i++){
                        atoms[i].GetComponent<Renderer>().enabled=true;
                    }
                    for (int i=0;i<bones_obs.Count;i++){
                        bones_obs[i].GetComponent<Renderer>().enabled=false;
                    }
                }
                for (int i=0;i<atoms.Count;i++){
                    atoms[i].transform.localScale=new Vector3(0.04f,0.04f,0.04f);
                }
                break;
                case 2:
                for (int i=0;i<atoms.Count;i++){
                    atoms[i].GetComponent<Renderer>().enabled=false;
                }
                if (pre_index==0){
                    for (int i=0;i<bonds_obs.Count;i++){
                        bonds_obs[i].GetComponent<Renderer>().enabled=false;
                    }
                }
                for (int i=0;i<bones_obs.Count;i++){
                    bones_obs[i].GetComponent<Renderer>().enabled=true;
                }
                break;
            }
        }

    }
}
