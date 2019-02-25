#!/bin/bash

# adjust these paths to reflect where you have downloaded the Nirvana data files
# In this example, we assume that the Cache, References, and SupplementaryDatabase
# folders have been downloaded into the NIRVANA_ROOT folder.

# In addition to downloading the Nirvana data files, make sure you have .NET Core 2.0
# installed on your computer:
# https://www.microsoft.com/net/download/core

NIRVANA_ROOT=/usr/local/Nirvana
NIRVANA_BIN=$NIRVANA_ROOT/bin/Release/netcoreapp2.0/Nirvana.dll
DATA_DIR=$NIRVANA_ROOT/Data
NIRVANA_TAG=v2.0.5

# just change this to GRCh38 if you want to set everything up for hg38
GENOME_ASSEMBLY=GRCh37
SA_VERSION=44
CACHE_VERSION=26
REF_VERSION=5

CACHE_DIR=$DATA_DIR/Cache/$CACHE_VERSION/$GENOME_ASSEMBLY
SA_DIR=$DATA_DIR/SupplementaryDatabase/$SA_VERSION
REF_DIR=$DATA_DIR/References/$REF_VERSION

CACHE_TGZ=$DATA_DIR/v${CACHE_VERSION}.tar.gz
SA_TGZ=$DATA_DIR/v${SA_VERSION}_${GENOME_ASSEMBLY}.tar.gz
REF_TGZ=$DATA_DIR/v${REF_VERSION}.tar.gz

CACHE_TEST=$CACHE_DIR/Ensembl.transcripts.ndb
SA_TEST=$SA_DIR/$GENOME_ASSEMBLY/chr1.nsa
REF_TEST=$REF_DIR/Homo_sapiens.${GENOME_ASSEMBLY}.Nirvana.dat
SOURCE_TEST=$NIRVANA_ROOT/Nirvana.sln

# =====================================================================

YELLOW='\033[1;33m'
RESET='\033[0m'

echo -ne $YELLOW
echo " _   _ _                             "
echo "| \ | (_)                            "
echo "|  \| |_ _ ____   ____ _ _ __   __ _ "
echo "| . \` | | '__\ \ / / _\` | '_ \ / _\` |"
echo "| |\  | | |   \ V / (_| | | | | (_| |"
echo "|_| \_|_|_|    \_/ \__,_|_| |_|\__,_|"
echo -e $RESET

# create the data directories
create_dir() {
    if [ ! -d $1 ]
    then
        mkdir -p $1
    fi
}
sudo_create_dir() {
    if [ ! -d $1 ]
    then
        sudo mkdir -p $1
    fi
}

sudo_create_dir $NIRVANA_ROOT
sudo_create_dir $CACHE_DIR
sudo_create_dir $REF_DIR
sudo_create_dir $SA_DIR

cd $NIRVANA_ROOT

# ==============================
# download all of the data files
# ==============================

URL_LIST=""

if [ ! -f $REF_TEST ] && [ ! -f $REF_TGZ ]
then
	URL_LIST="$URL_LIST http://illumina-annotation.s3.amazonaws.com/References/v${REF_VERSION}.tar.gz"
fi

if [ ! -f $CACHE_TEST ] && [ ! -f $CACHE_TGZ ]
then
    URL_LIST="$URL_LIST http://illumina-annotation.s3.amazonaws.com/Cache/v${CACHE_VERSION}.tar.gz"
fi

if [ ! -f $SA_TEST ] && [ ! -f $SA_TGZ ]
then
    URL_LIST="$URL_LIST http://illumina-annotation.s3.amazonaws.com/SA/$SA_VERSION/v${SA_VERSION}_${GENOME_ASSEMBLY}.tar.gz"
fi

if [ ! -z "$URL_LIST" ]
then
    echo -n "- downloading up to ~27 GB of data files in parallel (this will probably take a while)... "
    echo $URL_LIST | xargs -n 1 -P 8 wget -qcP Data
    echo "finished."
fi

# =====================
# unpack the data files
# =====================

unpack_file() {
    if [ ! -f $4 ]
    then
	pushd $2 > /dev/null
	echo -n "- unpacking $1 files... "
	tar xfz $3
	echo "finished."
	popd > /dev/null
	rm $3
    fi
}

unpack_file "reference" $DATA_DIR $REF_TGZ $REF_TEST
unpack_file "cache" $DATA_DIR $CACHE_TGZ $CACHE_TEST
unpack_file "supplementary annotation $GENOME_ASSEMBLY" $SA_DIR $SA_TGZ $SA_TEST

# download the Nirvana source
if [ ! -f $SOURCE_TEST ]
then
    # need to dance a little bit since git will complain this is a non-empty directory
    git init
    git remote add origin https://github.com/Lucioric2000/Nirvana.git
    git fetch
    git checkout $NIRVANA_TAG
fi

# =============
# build Nirvana
# =============

if [ ! -f $NIRVANA_BIN ]
then
    pushd Nirvana > /dev/null
    dotnet build -c Release
    popd > /dev/null
fi

# ==============================
# run Nirvana on a test VCF file
# ==============================

# download a test vcf file
if [ ! -f HiSeq.10000.vcf ]
then
    wget https://github.com/samtools/htsjdk/raw/master/src/test/resources/htsjdk/variant/HiSeq.10000.vcf
fi

# analyze it with Nirvana
echo dotnet $NIRVANA_BIN -c $CACHE_DIR/Ensembl --sd $SA_DIR/$GENOME_ASSEMBLY -r $REF_TEST -i HiSeq.10000.vcf -o HiSeq.10000.annotated

#CAAVEATS:
#I needed too conver the nirvana dir to belong to my user with the command:
#sudo chown lucioric:lucioric -R /usr/local/Nirvana/