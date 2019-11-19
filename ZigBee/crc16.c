#define CRC_POLY 0xa001 //CRC-16，多项式0x8005；0xa001为它的倒序

unsigned short crc16(unsigned char *data_p, unsigned char length)
{
    unsigned char i, j;
    unsigned short crc = 0xffff; //初始值0xffff

    for (i = 0; i < length; ++i)
    {
        crc ^= (unsigned short)(0xff & data_p[i]);
        for (j = 0; j < 8; j++)
        {
            if (crc & 0x0001)
                crc = (crc >> 1) ^ CRC_POLY;
            else
                crc >>= 1;
        }
    }
    // crc = (crc << 8) | (crc >> 8 & 0xff);
    return crc;
}

int main()
{
    unsigned char data[8] = {0xaa, 0x00, 0x00, 0x0b, 0x0b, 0x01, 0x00, 0x08};
    printf("%x\n", crc16(data, 8));
}