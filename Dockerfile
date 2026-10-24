FROM ubuntu:22.04

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y \
    curl wget git python3 python3-pip unzip xvfb \
    libgtk-3-0 libnss3 libasound2 libgbm1 libxss1 libxtst6 \
    libgl1 libgl1-mesa-dri libglu1-mesa libcurl4 ca-certificates \
    fonts-dejavu-core && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

WORKDIR /workspace

COPY . /workspace/DetectiveGame

RUN chmod +x /workspace/DetectiveGame/*.sh 2>/dev/null; true

CMD ["/bin/bash"]
